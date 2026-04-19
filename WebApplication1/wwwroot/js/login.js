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