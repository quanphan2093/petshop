
    const decreaseBtn = document.querySelectorAll('.decreaseBtn');
    const increaseBtn = document.querySelectorAll('.increaseBtn');
    const quantityInputs = document.querySelectorAll('.quantityInput');

    decreaseBtn.forEach((btn, index) => {
        btn.addEventListener('click', (event) => {
            event.preventDefault();
            const quantityInput = quantityInputs[index];
            let currentValue = parseInt(quantityInput.value) || 1;
            const minValue = parseInt(quantityInput.min) || 1;

            if (currentValue > minValue) {
                quantityInput.value = currentValue - 1;
            }
        });
    });

    increaseBtn.forEach((btn, index) => {
        btn.addEventListener('click', (event) => {
            event.preventDefault();
            const quantityInput = quantityInputs[index];
            let currentValue = parseInt(quantityInput.value) || 1;
            quantityInput.value = currentValue + 1;
        });
    });
