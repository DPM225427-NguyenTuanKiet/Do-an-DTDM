let revenueChart;

function formatNumber(num) {
    return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
}

async function loadChart(type = 'day') {
    const res = await fetch(`/Admin/ThongKe/GetStatsData?type=${type}`);
    const data = await res.json();
    if (revenueChart) revenueChart.destroy();
    const ctx = document.getElementById('revenueChart').getContext('2d');
    revenueChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: data.monthLabels,
            datasets: [{
                label: 'Doanh thu (VNĐ)',
                data: data.monthlyRevenues,
                backgroundColor: 'rgba(230, 126, 34, 0.6)',
                borderColor: '#e67e22',
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                tooltip: { callbacks: { label: (ctx) => formatNumber(ctx.raw) + ' VNĐ' } }
            },
            scales: { y: { ticks: { callback: (val) => formatNumber(val) } } }
        }
    });
    // Update summary cards
    document.getElementById('totalRevenue').innerText = formatNumber(data.totalRevenue) + ' VNĐ';
    document.getElementById('totalOrders').innerText = data.totalOrders;
    document.getElementById('pendingOrders').innerText = data.pendingOrders;
    document.getElementById('deliveredOrders').innerText = data.deliveredOrders;
    // Update top products
    const topList = document.getElementById('topProductsList');
    topList.innerHTML = data.topProducts.map(p => `
                <li class="list-group-item d-flex justify-content-between align-items-center">
                    ${p.name}
                    <span class="badge bg-primary rounded-pill">${p.sold} cái</span>
                </li>
            `).join('');
}

document.getElementById('chartType').addEventListener('change', function () {
    loadChart(this.value);
});

loadChart('day');
setInterval(() => loadChart(document.getElementById('chartType').value), 30000);
// Auto-refresh dashboard data every 30 seconds
let refreshInterval;

function startAutoRefresh() {
    if (refreshInterval) clearInterval(refreshInterval);
    refreshInterval = setInterval(() => {
        if (window.location.pathname.includes('/Admin/ThongKe')) {
            refreshStats();
        }
    }, 30000);
}

function refreshStats() {
    fetch('/Admin/ThongKe/GetStatsData')
        .then(res => res.json())
        .then(data => {
            // Update cards
            document.getElementById('totalRevenue').innerText = formatNumber(data.totalRevenue) + ' VNĐ';
            document.getElementById('totalOrders').innerText = data.totalOrders;
            document.getElementById('pendingOrders').innerText = data.pendingOrders;
            document.getElementById('deliveredOrders').innerText = data.deliveredOrders;
            // Update chart
            if (window.revenueChart) {
                window.revenueChart.data.datasets[0].data = data.monthlyRevenues;
                window.revenueChart.update();
            }
            // Update top products
            const topList = document.getElementById('topProductsList');
            if (topList) {
                topList.innerHTML = data.topProducts.map(p => `
                    <li class="list-group-item d-flex justify-content-between align-items-center">
                        ${p.name}
                        <span class="badge bg-primary rounded-pill">${p.sold} cái</span>
                    </li>
                `).join('');
            }
        });
}

function formatNumber(num) {
    return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
}

// Search and filter for product management
function filterProducts() {
    const keyword = document.getElementById('searchKeyword').value.toLowerCase();
    const category = document.getElementById('filterCategory').value;
    const rows = document.querySelectorAll('#productTable tbody tr');
    rows.forEach(row => {
        const name = row.cells[1].innerText.toLowerCase();
        const catId = row.cells[4].innerText;
        let show = true;
        if (keyword && !name.includes(keyword)) show = false;
        if (category && category !== 'all' && catId !== category) show = false;
        row.style.display = show ? '' : 'none';
    });
}

// Confirm delete
function confirmDelete(url) {
    if (confirm('Bạn có chắc chắn muốn xóa?')) {
        window.location.href = url;
    }
}

// Initialize chart
function initRevenueChart(labels, data) {
    const ctx = document.getElementById('revenueChart').getContext('2d');
    window.revenueChart = new Chart(ctx, {
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

// Run on page load
document.addEventListener('DOMContentLoaded', () => {
    startAutoRefresh();
    // Bind search/filter events
    const searchInput = document.getElementById('searchKeyword');
    if (searchInput) {
        searchInput.addEventListener('keyup', filterProducts);
        document.getElementById('filterCategory')?.addEventListener('change', filterProducts);
    }
});
// Chọn tất cả checkbox
$('#selectAll').on('change', function () {
    $('.rowCheckbox').prop('checked', $(this).prop('checked'));
    toggleDeleteBtn();
});
$('.rowCheckbox').on('change', function () {
    toggleDeleteBtn();
});
function toggleDeleteBtn() {
    var checked = $('.rowCheckbox:checked').length > 0;
    $('#deleteSelectedBtn').prop('disabled', !checked);
}
toggleDeleteBtn();
// Hàm format số
function formatNumber(num) {
    return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
}

// Lấy dữ liệu thống kê từ API
async function loadStats() {
    try {
        const response = await fetch('/Admin/ThongKe/GetStatsData');
        const data = await response.json();

        document.getElementById('totalRevenue').innerText = formatNumber(data.totalRevenue) + ' VNĐ';
        document.getElementById('totalOrders').innerText = data.totalOrders;
        document.getElementById('pendingOrders').innerText = data.pendingOrders;
        document.getElementById('deliveredOrders').innerText = data.deliveredOrders;

        // Cập nhật danh sách top sản phẩm
        const topList = document.getElementById('topProductsList');
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

        // Khởi tạo hoặc cập nhật biểu đồ
        if (window.revenueChart) {
            window.revenueChart.data.datasets[0].data = data.monthlyRevenues;
            window.revenueChart.update();
        } else {
            initRevenueChart(data.monthLabels, data.monthlyRevenues);
        }
    } catch (error) {
        console.error('Lỗi tải dữ liệu:', error);
    }
}

// Khởi tạo biểu đồ doanh thu
function initRevenueChart(labels, data) {
    const ctx = document.getElementById('revenueChart').getContext('2d');
    window.revenueChart = new Chart(ctx, {
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
            maintainAspectRatio: true,
            plugins: {
                tooltip: {
                    callbacks: {
                        label: (ctx) => formatNumber(ctx.raw) + ' VNĐ'
                    }
                }
            },
            scales: {
                y: {
                    ticks: {
                        callback: (val) => formatNumber(val)
                    }
                }
            }
        }
    });
}

// Tự động làm mới mỗi 30 giây
setInterval(loadStats, 30000);

// Tải dữ liệu lần đầu
loadStats();