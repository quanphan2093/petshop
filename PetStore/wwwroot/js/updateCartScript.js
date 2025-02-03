function decreaseQuantity(index, quantityInputs) {
    const quantityInput = quantityInputs[index];
    let currentValue = parseInt(quantityInput.value) || 1;
    const minValue = 1;

    if (currentValue > minValue) {
        quantityInput.value = currentValue - 1;
        return quantityInput.value;
    }

    return 0;
}

function updateTotal(totalPriceElement) {
    let total = 0;
    document.querySelectorAll('.cart-item').forEach(cartItem => {
        let quantity = parseInt(cartItem.querySelector('.quantityInput').value) || 0;
        let price = parseInt(cartItem.querySelector('.fw-bold b').innerText.replace(/\D/g, '')) || 0;
        total += quantity * price;
    });

    totalPriceElement.innerText = total.toLocaleString('en-US') + " VND";
}

function updateEventListeners() {
    const decreaseBtns = document.querySelectorAll('.decreaseBtn');
    const totalPriceElement = document.querySelector('.total-price');


    for (let i = 0; i < decreaseBtns.length; i++) {
        decreaseBtns[i].addEventListener("click", function (event) {
            const form = document.getElementById("cart");
            let quantityInputs = document.querySelectorAll("input[name='quantity']");
            let cartIdElements = document.querySelectorAll("input[name='cartId']");

            let newQuantity = decreaseQuantity(i, quantityInputs);
            quantityInputs[i].value = newQuantity;



            const formData = new FormData();
            formData.append("__RequestVerificationToken", document.querySelector("input[name='__RequestVerificationToken']").value);

            cartIdElements.forEach((cartElement, idx) => {
                formData.append(`cartIds[${idx}]`, cartElement.value);
            });

            quantityInputs.forEach((quantityElement, idx) => {
                formData.append(`quantities[${idx}]`, quantityElement.value);
            });

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
            updateTotal(totalPriceElement);
            if (newQuantity == 0) {
                const cartItem = decreaseBtns[i].closest('.cart-item');
                if (cartItem) {
                    cartItem.remove();
                    setTimeout(() => {
                        window.location.reload();
                    }, 500);
                }
            }
        });
    }

    const increaseBtns = document.querySelectorAll('.increaseBtn');
    increaseBtns.forEach((btn, index) => {
        btn.addEventListener("click", function (event) {
            const form = document.getElementById("cart");
            const cartIdElements = document.querySelectorAll("input[name='cartId']");
            const quantityInputs = document.querySelectorAll("input[name='quantity']");

            const quantityInput = quantityInputs[index];
            let currentValue = parseInt(quantityInput.value) || 1;
            quantityInput.value = currentValue + 1;
            quantityInputs[index] = quantityInput.value;

            const formData = new FormData();
            formData.append("__RequestVerificationToken", document.querySelector("input[name='__RequestVerificationToken']").value);
            cartIdElements.forEach((cartElement, idx) => {
                formData.append(`cartIds[${idx}]`, cartElement.value);
            });

            quantityInputs.forEach((quantityElement, idx) => {
                formData.append(`quantities[${idx}]`, quantityElement.value);
            });

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
            updateTotal(totalPriceElement);
        });
    });

    const inputQuantity = document.querySelectorAll('.quantityInput');
    for (let i = 0; i < inputQuantity.length; i++) {
        inputQuantity[i].addEventListener("change", function (event) {
            const form = document.getElementById("cart");
            const cartIdElements = document.querySelectorAll("input[name='cartId']");
            const quantityInputs = document.querySelectorAll("input[name='quantity']");

            const formData = new FormData();
            formData.append("__RequestVerificationToken", document.querySelector("input[name='__RequestVerificationToken']").value);
            cartIdElements.forEach((cartElement, idx) => {
                formData.append(`cartIds[${idx}]`, cartElement.value);
            });

            quantityInputs.forEach((quantityElement, idx) => {
                formData.append(`quantities[${idx}]`, quantityElement.value);
            });

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
            updateTotal(totalPriceElement);
            if (inputQuantity[i].value == 0) {
                const cartItem = inputQuantity[i].closest('.cart-item');
                if (cartItem) {
                    cartItem.remove();
                    setTimeout(() => {
                        window.location.reload();
                    }, 500);
                }
            }
        });
    };
}

updateEventListeners();