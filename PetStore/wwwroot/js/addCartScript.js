document.addEventListener('DOMContentLoaded', () => {
    document.getElementById("addToCartBtn").addEventListener("click", function () {
        const form = document.getElementById("cartForm");
        const productId = document.getElementById("productId").value;
        const quantity = document.querySelector(".quantityInput").value;

        const formData = new FormData();
        formData.append("__RequestVerificationToken", document.querySelector("input[name='__RequestVerificationToken']").value);
        formData.append("productId", productId);
        formData.append("quantity", quantity);

        fetch(form.action, {
            method: "POST",
            body: formData
        })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            if (data.message === "Vui lòng login trước!") {
                console.log("login")
                window.location.href = "/login";
            }
        })
        .catch(error => {
            console.error("Error:", error);
        });
    });
});