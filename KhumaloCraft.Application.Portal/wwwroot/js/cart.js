function updateCartCount(id, count) {
    document.getElementById(`cart-count-${id}`).textContent = count;
}

function addToCart(craftworkId) {
    var button = document.getElementById(`add-to-cart-${craftworkId}`);
    button.disabled = true;

    var payload = {
        craftworkId
    };

    $.ajax({
        url: 'cart/add-item',
        type: 'POST',
        data: JSON.stringify(payload),
        contentType: 'application/json',
        processData: false,
        success: function (response) {
            if (response.success) {
                button.disabled = false;
                button.classList.add('hidden-kc');

                var controls = document.getElementById(`cart-controls-${craftworkId}`);
                controls.classList.remove('hidden-kc');

                var countElement = document.getElementById(`item-count-${craftworkId}`);
                if (countElement) {
                    countElement.textContent = 1;
                }
            }
            else {
                alert(response.message);
            }
        },
        error: function (error) {
            console.error('Server Error: ', error);
        }
    });
}

function incrementCartItem(craftworkId) {
    let countElement = document.getElementById(`item-count-${craftworkId}`);

    var payload = {
        craftworkId
    };

    $.ajax({
        url: 'cart/increment-cart-item',
        type: 'POST',
        data: JSON.stringify(payload),
        contentType: 'application/json',
        processData: false,
        success: function (response) {
            if (response.success) {
                countElement.textContent = response.quantity;
            }
            else {
                alert(response.message);
            }
        },
        error: function (error) {
            console.error('Server Error: ', error);
        }
    });
}

function decrementCartItem(craftworkId) {
    let countElement = document.getElementById(`item-count-${craftworkId}`);
    let controls = document.getElementById(`cart-controls-${craftworkId}`);

    var payload = {
        craftworkId
    };

    $.ajax({
        url: 'cart/decrement-cart-item',
        type: 'POST',
        data: JSON.stringify(payload),
        contentType: 'application/json',
        processData: false,
        success: function (response) {
            if (response.success) {
                if (response.quantity === 0) {
                    var addToCartButton = document.getElementById(`add-to-cart-${craftworkId}`);

                    controls.classList.add('hidden-kc');

                    addToCartButton.classList.remove('hidden-kc');
                }
                else {
                    countElement.textContent = response.quantity;
                }
            }
            else {
                alert(response.message);
            }
        },
        error: function (error) {
            console.error('Server Error: ', error);
        }
    });
}