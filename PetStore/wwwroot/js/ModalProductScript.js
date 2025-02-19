const editProductModal = document.getElementById('editProduct');

editProductModal.addEventListener('show.bs.modal', function (event) {
    const button = event.relatedTarget; // Button that triggered the modal

    // Lấy dữ liệu từ các thuộc tính data-*
    const productId = button.getAttribute('data-productid');
    const productName = button.getAttribute('data-productname');
    let details = button.getAttribute('data-details');
    details = details.replace(/\\r\\n/g, "\n").replace(/\\n/g, "\n");
    const price = button.getAttribute('data-price');
    const image = button.getAttribute('data-image');
    let discount = button.getAttribute('data-discount');
    let size = button.getAttribute('data-size');
    let unitInStock = button.getAttribute('data-unitinstock');
    let categoryId = button.getAttribute('data-categoryid');
    let status = button.getAttribute('data-status');
    let shopId = button.getAttribute('data-shopId');

    discount = discount ? discount : 0;
    size = size ? size : null;
    unitInStock = unitInStock ? unitInStock : 0;
     
    // Điền dữ liệu vào các input của modal
    editProductModal.querySelector('input[name="productId"]').value = productId;
    editProductModal.querySelector('input[name="productName"]').value = productName;
    editProductModal.querySelector('textarea[name="productDetail"]').value = details;
    editProductModal.querySelector('input[name="proprice"]').value = price;
    editProductModal.querySelector('input[name="prodiscount"]').value = discount;
    let sizeInput = editProductModal.querySelector('input[name="prosize"]');
    sizeInput.value = size;
    sizeInput.placeholder = size === null ? 'null' : '';
    editProductModal.querySelector('input[name="prounitInStock"]').value = unitInStock;
    let categorySelect = editProductModal.querySelector('select[name="productCate"]');
    categorySelect.value = categoryId;
    editProductModal.querySelector('select[name="prostatus"]').value = status;
    editProductModal.querySelector('select[name="shop"]').value = shopId;
});
