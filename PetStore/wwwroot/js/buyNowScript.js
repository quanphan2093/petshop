const btnBuyNow = document.querySelector(".buy-now");
btnBuyNow.addEventListener("click", function () {
    const quantity = document.querySelector(".quantityInput").value;
    const productId = document.getElementById("productId").value;
    let unitInStockElement = document.getElementById("UnitInStock").textContent.trim().split(" ")[0];

    let stockNumber = Number(unitInStockElement);

    if (stockNumber <= 0) {
        let errorDiv = document.getElementById("errorMessage");
        errorDiv.innerText = "Số lượng trong kho đã hết hoặc nhỏ hơn số lượng bạn mua!";
        errorDiv.style.display = "block";
    }
    else {
        window.location = "/Checkout?productId=" + productId + "&quantity=" + quantity;
    }
});