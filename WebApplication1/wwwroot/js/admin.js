// ==========================================
// FILE ADMIN.JS (ĐÃ TỐI ƯU VÀ GỘP CODE TRÙNG)
// ==========================================

// 1. BIẾN TOÀN CỤC
var revenueChart;
var refreshInterval;

// 2. HÀM TIỆN ÍCH
// Định dạng số tiền (Ví dụ: 1000000 -> 1.000.000)
function formatNumber(num) {
    if (num === null || num === undefined) return "0";
    return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
}

// Cảnh báo xác nhận xóa
function confirmDelete(url) {
    if (confirm('Bạn có chắc chắn muốn xóa mục này?')) {
        window.location.href = url;
    }
}

// 3. HÀM QUẢN LÝ BIỂU ĐỒ (CHART)
function initRevenueChart(labels, data) {
    const canvas = document.getElementById('revenueChart');
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    revenueChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Doanh thu (VNĐ)',
                data: data,
                backgroundColor: 'rgba(230, 126, 34, 0.6)',
                borderColor: '#e67e22',
                borderWidth: 1,
                borderRadius: 6
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { position: 'top' },
                tooltip: { callbacks: { label: (ctx) => formatNumber(ctx.raw) + ' VNĐ' } }
            },
            scales: {
                y: { ticks: { callback: (val) => formatNumber(val) } }
            }
        }
    });
}

// 4. HÀM QUẢN LÝ THỐNG KÊ (DASHBOARD)
// Đã gộp loadChart, loadStats và refreshStats thành 1 hàm duy nhất
async function loadDashboardStats(type = 'day') {
    try {
        const response = await fetch(`/Admin/ThongKe/GetStatsData?type=${type}`);
        if (!response.ok) return;
        const data = await response.json();

        // 4.1 Cập nhật các thẻ số liệu
        const elRev = document.getElementById('totalRevenue');
        if (elRev) elRev.innerText = formatNumber(data.totalRevenue) + ' VNĐ';

        const elOrd = document.getElementById('totalOrders');
        if (elOrd) elOrd.innerText = data.totalOrders;

        const elPend = document.getElementById('pendingOrders');
        if (elPend) elPend.innerText = data.pendingOrders;

        const elDeliv = document.getElementById('deliveredOrders');
        if (elDeliv) elDeliv.innerText = data.deliveredOrders;

        // 4.2 Cập nhật danh sách Top Sản phẩm
        const topList = document.getElementById('topProductsList');
        if (topList) {
            if (data.topProducts && data.topProducts.length > 0) {
                topList.innerHTML = data.topProducts.map(p => `
                    <li class="list-group-item d-flex justify-content-between align-items-center">
                        ${p.name}
                        <span class="badge bg-primary rounded-pill">${p.sold} cái</span>
                    </li>
                `).join('');
            } else {
                topList.innerHTML = '<li class="list-group-item text-center">Chưa có dữ liệu</li>';
            }
        }

        // 4.3 Cập nhật Biểu đồ
        const chartCanvas = document.getElementById('revenueChart');
        if (chartCanvas) {
            if (revenueChart) {
                revenueChart.data.labels = data.monthLabels;
                revenueChart.data.datasets[0].data = data.monthlyRevenues;
                revenueChart.update();
            } else {
                initRevenueChart(data.monthLabels, data.monthlyRevenues);
            }
        }
    } catch (error) {
        console.error('Lỗi tải dữ liệu thống kê:', error);
    }
}

// 5. HÀM QUẢN LÝ TÌM KIẾM & BẢNG (SẢN PHẨM/TÀI KHOẢN)
// Lọc sản phẩm trực tiếp trên bảng
function filterProducts() {
    const keywordEl = document.getElementById('searchKeyword');
    const categoryEl = document.getElementById('filterCategory');
    if (!keywordEl) return;

    const keyword = keywordEl.value.toLowerCase();
    const category = categoryEl ? categoryEl.value : 'all';
    const rows = document.querySelectorAll('#productTable tbody tr');

    rows.forEach(row => {
        if (row.cells.length < 5) return;
        const name = row.cells[1].innerText.toLowerCase();
        const catId = row.cells[4].innerText;
        let show = true;

        if (keyword && !name.includes(keyword)) show = false;
        if (category && category !== 'all' && catId !== category) show = false;
        row.style.display = show ? '' : 'none';
    });
}

// Bật/tắt nút Xóa Hàng Loạt
function toggleDeleteBtn() {
    if (typeof $ !== 'undefined') {
        var checked = $('.rowCheckbox:checked').length > 0;
        $('#deleteSelectedBtn').prop('disabled', !checked);
    }
}

// ==========================================
// KHỞI CHẠY CÁC SỰ KIỆN KHI TRANG TẢI XONG
// ==========================================
document.addEventListener('DOMContentLoaded', () => {

    // --- KHU VỰC DASHBOARD THỐNG KÊ ---
    // Chỉ chạy load thống kê nếu đang ở trang Dashboard
    if (document.getElementById('totalRevenue') || document.getElementById('revenueChart')) {
        const chartTypeElement = document.getElementById('chartType');
        let currentType = chartTypeElement ? chartTypeElement.value : 'day';

        // Tải dữ liệu lần đầu
        loadDashboardStats(currentType);

        // Đổi loại biểu đồ (Ngày/Tháng/Năm) nếu có hộp chọn
        if (chartTypeElement) {
            chartTypeElement.addEventListener('change', function () {
                currentType = this.value;
                loadDashboardStats(currentType);
            });
        }

        // Xóa vòng lặp cũ (nếu có) và tạo vòng lặp mới mỗi 30s
        if (refreshInterval) clearInterval(refreshInterval);
        refreshInterval = setInterval(() => loadDashboardStats(currentType), 30000);
    }


    // --- KHU VỰC TÌM KIẾM BẢNG ---
    const searchInput = document.getElementById('searchKeyword');
    if (searchInput) {
        searchInput.addEventListener('keyup', filterProducts);
        const filterCat = document.getElementById('filterCategory');
        if (filterCat) filterCat.addEventListener('change', filterProducts);
    }


    // --- KHU VỰC CHECKBOX XÓA NHIỀU (Dùng jQuery) ---
    if (typeof $ !== 'undefined') {
        $('#selectAll').on('change', function () {
            $('.rowCheckbox').prop('checked', $(this).prop('checked'));
            toggleDeleteBtn();
        });
        $('.rowCheckbox').on('change', toggleDeleteBtn);

        // Chạy lần đầu để khóa nút Xóa nếu chưa chọn ai
        toggleDeleteBtn();
    }
});