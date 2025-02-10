const btnBuyNow = document.querySelector(".buy-now");
btnBuyNow.addEventListener("click", function () {
    const quantity = document.querySelector(".quantityInput").value;
    const productId = document.getElementById("productId").value;
    window.location = "/Checkout?productId=" + productId + "&quantity=" + quantity;
});