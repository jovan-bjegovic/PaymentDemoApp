const stripe = Stripe(document.body.dataset.publishableKey );

const elements = stripe.elements();

const style = {
    base: {
        fontSize: "16px",
        color: "#32325d",
        "::placeholder": { color: "#a0aec0" }
    }
};

const cardNumber = elements.create("cardNumber", { style });
cardNumber.mount("#card-number-element");

const cardExpiry = elements.create("cardExpiry", { style });
cardExpiry.mount("#card-exp-element");

const cardCvc = elements.create("cardCvc", { style });
cardCvc.mount("#card-cvc-element");

const form = document.getElementById('payment-form');
form.addEventListener('submit', async (e) => {
    e.preventDefault();

    const response = await fetch('/api/payments/create-payment-intent', {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({ amount: 1000 })
    });

    const data = await response.json();

    const result = await stripe.confirmCardPayment(data.clientSecret, {
        payment_method: {
            card: cardNumber,
            billing_details: {
                name: document.getElementById('name').value,
                email: document.getElementById('email').value,
                address: { country: document.getElementById('country').value }
            }
        }
    });

    if (result.error) {
        window.location.href = "/checkout-cancel";
    } else if (result.paymentIntent.status === 'succeeded') {
        window.location.href = "/checkout-success";
    }
});
