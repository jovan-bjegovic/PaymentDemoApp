const style = {
    base: {
        fontSize: "16px",
        color: "#32325d",
        "::placeholder": { color: "#a0aec0" }
    }
};

const countryToCurrency = {
    "RS": "EUR",
    "US": "USD",
    "GB": "GBP",
    "DE": "EUR",
    "FR": "EUR",
    "CA": "CAD",
    "AU": "AUD"
};

let baseAmount = 10.00;

const countrySelect = document.getElementById("country");
const currencyInput = document.getElementById("currency");
const amountDisplay = document.getElementById("amount-display");

function updateCurrencyAndAmount() {
    const currency = countryToCurrency[countrySelect.value] || "eur";
    currencyInput.value = currency;
    
    let displayAmount = baseAmount.toFixed(2);
    amountDisplay.textContent = `${displayAmount} ${currency.toUpperCase()}`;
}

currencyInput.value = countryToCurrency[countrySelect.value] || "eur";
countrySelect.addEventListener("change", updateCurrencyAndAmount);
updateCurrencyAndAmount();

const stripePayment = Stripe(document.body.dataset.publishableKey);
const elements = stripePayment.elements();

const cardNumber = elements.create("cardNumber", { style });
cardNumber.mount("#card-number-element");

const cardExpiry = elements.create("cardExpiry", { style });
cardExpiry.mount("#card-exp-element");

const cardCvc = elements.create("cardCvc", { style });
cardCvc.mount("#card-cvc-element");

const form = document.getElementById('payment-form');
form.addEventListener('submit', async (e) => {
    e.preventDefault();

    const currency = (currencyInput.value || "usd").toLowerCase();
    let amountToSend = currency === "rsd" ? Math.round(baseAmount * 10000) : Math.round(baseAmount * 100);

    const response = await fetch('/api/payments/create-payment', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            amount: amountToSend,
            currency: currency,
            name: document.getElementById('name').value,
            email: document.getElementById('email').value,
            country: document.getElementById('country').value
        })
    });

    const text = await response.text();

    if (!response.ok) {
        try {
            const jsonErr = JSON.parse(text);
            console.error("Server error:", jsonErr);
        } catch (err) {
            console.error("Server returned:", text);
        }
        window.location.href = "/checkout-cancel";
        
        return;
    }

    let data = null;
    try {
        data = text ? JSON.parse(text) : null;
    } catch {
        window.location.href = "/checkout-cancel";
        return;
    }

    if (!data || !data.clientSecret) {
        console.error("Missing clientSecret in response:", data);
        window.location.href = "/checkout-cancel";
        return;
    }

    const result = await stripePayment.confirmCardPayment(data.clientSecret, {
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
    } else if (result.paymentIntent && result.paymentIntent.status === 'succeeded') {
        window.location.href = "/checkout-success";
    } else {
        window.location.href = "/checkout-cancel";
    }
});

