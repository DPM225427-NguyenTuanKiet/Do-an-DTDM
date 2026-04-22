// features.js - Xử lý giỏ hàng, yêu thích, mua ngay, bình luận, sản phẩm liên quan
// Phiên bản gọn, không trùng lặp, có console.log để debug

// ==================== GIỎ HÀNG ====================
function addToCart(productId, quantity) {
    var token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    console.log("addToCart:", productId, quantity);
    fetch('/GioHang/AddToCart', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded', 'RequestVerificationToken': token || '' },
        body: new URLSearchParams({ id: productId, quantity: quantity })
    })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                Swal.fire({ icon: 'success', title: 'Thành công!', text: 'Đã thêm vào giỏ', timer: 1500, toast: true, position: 'bottom-end', showConfirmButton: false });
                updateCartBadge(data.cartCount);
            } else {
                Swal.fire({ icon: 'error', title: 'Lỗi!', text: data.message, timer: 1500, toast: true, position: 'bottom-end', showConfirmButton: false });
            }
        })
        .catch(() => Swal.fire('Lỗi', 'Không thể kết nối', 'error'));
}

function updateCartBadge(count) {
    var badge = document.querySelector('.cart-badge');
    if (badge) {
        badge.textContent = count;
        badge.style.display = count > 0 ? 'inline-block' : 'none';
    } else {
        var cartLink = document.querySelector('a.cart-icon, a[href*="GioHang"]');
        if (cartLink && count > 0) {
            var span = document.createElement('span');
            span.className = 'cart-badge';
            span.textContent = count;
            cartLink.appendChild(span);
        }
    }
}

function updateQuantity(productId, quantity) {
    if (quantity <= 0) { removeFromCart(productId); return; }
    var token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    fetch('/GioHang/UpdateQuantity', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded', 'RequestVerificationToken': token || '' },
        body: new URLSearchParams({ productId: productId, quantity: quantity })
    })
        .then(res => res.json())
        .then(data => { if (data.success) location.reload(); else Swal.fire('Lỗi', data.message, 'error'); });
}

function removeFromCart(productId) {
    Swal.fire({
        title: 'Xác nhận xóa?',
        text: "Bạn có chắc muốn xóa sản phẩm này khỏi giỏ hàng?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        confirmButtonText: 'Xóa',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            var token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            fetch('/GioHang/RemoveFromCart', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded', 'RequestVerificationToken': token || '' },
                body: new URLSearchParams({ productId: productId })
            })
                .then(res => res.json())
                .then(data => { if (data.success) location.reload(); else Swal.fire('Lỗi', data.message, 'error'); });
        }
    });
}

function clearCart() {
    var token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    fetch('/GioHang/ClearCart', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded', 'RequestVerificationToken': token || '' }
    })
        .then(res => res.json())
        .then(data => { if (data.success) location.reload(); else Swal.fire('Lỗi', 'Không thể xóa giỏ hàng', 'error'); });
}

function confirmClearCart() {
    Swal.fire({
        title: 'Xác nhận xóa?',
        text: "Toàn bộ sản phẩm trong giỏ sẽ bị xóa.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        confirmButtonText: 'Đồng ý',
        cancelButtonText: 'Hủy'
    }).then((result) => { if (result.isConfirmed) clearCart(); });
}

// ==================== YÊU THÍCH ====================
function addToFavorite(productId, btnElement) {
    var token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    console.log("addToFavorite:", productId);
    fetch('/YeuThich/AddToFavorite', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded', 'RequestVerificationToken': token || '' },
        body: new URLSearchParams({ productId: productId })
    })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                Swal.fire({ icon: 'success', title: 'Thành công!', text: data.message, timer: 1500, toast: true, position: 'bottom-end', showConfirmButton: false });
                if (btnElement) $(btnElement).find('i').removeClass('far').addClass('fas').css('color', 'red');
            } else {
                Swal.fire({ icon: 'error', title: 'Lỗi!', text: data.message, timer: 1500, toast: true, position: 'bottom-end', showConfirmButton: false });
            }
        });
}

function removeFromFavorite(productId, btnElement) {
    var token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    fetch('/YeuThich/RemoveFromFavorite', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded', 'RequestVerificationToken': token || '' },
        body: new URLSearchParams({ productId: productId })
    })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                Swal.fire({ icon: 'success', title: 'Đã xóa!', text: data.message, timer: 1500, toast: true, position: 'bottom-end', showConfirmButton: false });
                if (btnElement) $(btnElement).find('i').removeClass('fas').addClass('far').css('color', '');
                if (window.location.pathname.includes('/YeuThich')) {
                    $(btnElement).closest('.col-md-3').remove();
                    if ($('.col-md-3').length === 0) location.reload();
                }
            } else {
                Swal.fire({ icon: 'error', title: 'Lỗi!', text: data.message, timer: 1500, toast: true, position: 'bottom-end', showConfirmButton: false });
            }
        });
}

// ==================== MUA NGAY ====================
function buyNow(productId, quantity) {
    var token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    fetch('/GioHang/AddToCart', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded', 'RequestVerificationToken': token || '' },
        body: new URLSearchParams({ id: productId, quantity: quantity })
    })
        .then(res => res.json())
        .then(data => {
            if (data.success) window.location.href = '/GioHang/Checkout';
            else Swal.fire('Lỗi', data.message, 'error');
        })
        .catch(() => Swal.fire('Lỗi', 'Có lỗi xảy ra', 'error'));
}

// ==================== BÌNH LUẬN & SẢN PHẨM LIÊN QUAN ====================
window.loadComments = function (page, productId) {
    $.get('/BinhLuan/GetComments', { productId: productId, page: page }, function (data) {
        $('#comment-list').html(data);
    });
};

window.loadRelatedProducts = function (productId) {
    $.get('/Home/RelatedProducts', { productId: productId }, function (data) {
        $('#related-products').html(data);
    });
};

window.submitComment = function (productId) {
    var content = $('#comment-content').val();
    var rating = $('#rating').val();
    if (!content) {
        Swal.fire('Lỗi', 'Vui lòng nhập nội dung', 'error');
        return;
    }
    $.post('/BinhLuan/Create', { productId: productId, content: content, rating: rating }, function (res) {
        if (res.success) {
            Swal.fire('Thành công', 'Cảm ơn bạn đã đánh giá', 'success');
            $('#comment-content').val('');
            loadComments(1, productId);
        } else {
            Swal.fire('Lỗi', res.message, 'error');
        }
    });
};

// ==================== KHỞI TẠO SỰ KIỆN (chỉ 1 lần) ====================
$(document).ready(function () {
    console.log("features.js ready");

    // Lấy productId từ các nút có data-id (ưu tiên nút submit-comment)
    var productId = $('#submit-comment').data('id');

    // Điều khiển số lượng (nếu có)
    if ($('#qty-minus').length) {
        $('#qty-minus').click(function () { var qty = parseInt($('#quantity').val()); if (qty > 1) $('#quantity').val(qty - 1); });
        $('#qty-plus').click(function () { var qty = parseInt($('#quantity').val()); if (qty < 99) $('#quantity').val(qty + 1); });
    }

    // Load bình luận & sản phẩm liên quan nếu có
    if ($('#comment-list').length && productId) loadComments(1, productId);
    if ($('#related-products').length && productId) loadRelatedProducts(productId);

    // Gán sự kiện cho các nút chính (dùng event delegation để đảm bảo cả dynamic content)
    $(document).on('click', '.add-to-cart', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var qty = $('#quantity').length ? $('#quantity').val() : 1;
        addToCart(id, qty);
    });
    $(document).on('click', '.buy-now', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var qty = $('#quantity').length ? $('#quantity').val() : 1;
        buyNow(id, qty);
    });
    $(document).on('click', '.add-to-favorite', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        addToFavorite(id, this);
    });
    $(document).on('click', '.remove-from-favorite', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        removeFromFavorite(id, this);
    });
    $(document).on('change', '.qty', function () {
        var id = $(this).data('id');
        var qty = $(this).val();
        if (qty < 1) qty = 1;
        updateQuantity(id, qty);
    });
    $(document).on('click', '.remove-item', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        removeFromCart(id);
    });
    $(document).on('click', '#clear-cart', function (e) {
        e.preventDefault();
        confirmClearCart();
    });
    $('#submit-comment').click(function () {
        var pid = $(this).data('id');
        submitComment(pid);
    });
});