document.addEventListener('DOMContentLoaded', () => {
    const decreaseBtn = document.getElementById('decreaseBtn');
    const increaseBtn = document.getElementById('increaseBtn');
    const quantityInput = document.getElementById('quantityInput');

    decreaseBtn.addEventListener('click', () => {
        let currentValue = parseInt(quantityInput.value) || 1;
        const minValue = parseInt(quantityInput.min) || 1;
        if (currentValue > minValue) {
            quantityInput.value = currentValue - 1;
        }
    });

    increaseBtn.addEventListener('click', () => {
        let currentValue = parseInt(quantityInput.value) || 1;
        quantityInput.value = currentValue + 1;
    });



    const thumbnails = document.querySelectorAll('.product-thumbnails img');
    const mainImage = document.getElementById('main-image');

    const updateThumbnails = () => {
        thumbnails.forEach(thumbnail => {
            if (thumbnail.src === mainImage.src) {
                thumbnail.classList.remove('dimmed');
            } else {
                thumbnail.classList.add('dimmed');
            }
        });
    };

    thumbnails.forEach(thumbnail => {
        thumbnail.addEventListener('click', () => {
            const newSrc = thumbnail.src;

            mainImage.classList.add('fade-out');
            setTimeout(() => {
                mainImage.src = newSrc;
                mainImage.classList.remove('fade-out');
                updateThumbnails();
            }, 200);
        });
    });

    updateThumbnails();
    $(document).ready(function () {
        $('#toggleButton').click(function () {
            $('#infoContent').fadeToggle(200);
        });
    });
    (function ($) {
        "use strict";

        // Product carousel
        $(".product-carousel").owlCarousel({
            autoplay: true,
            smartSpeed: 1000,
            margin: 20,
            dots: false,
            loop: true,
            nav: true,
            navText: [
                '<i class="bi bi-chevron-left"></i>',
                '<i class="bi bi-chevron-right"></i>'
            ],
            responsive: {
                0: {
                    items: 1
                },
                768: {
                    items: 2
                },
                992: {
                    items: 3
                },
                1200: {
                    items: 4
                }
            }
        });

    })(jQuery);
});



