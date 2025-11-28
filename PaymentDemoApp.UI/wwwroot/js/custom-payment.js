const stripe = Stripe(document.body.dataset.publishableKey);

const elements = stripe.elements();

const style = {
    base: {
        fontSize: "16px",
        color: "#32325d",
        "::placeholder": { color: "#a0aec0" }
    }
};

const countryToCurrency = {
    "RS": "rsd",
    "US": "usd",
    "GB": "gbp",
    "DE": "eur",
    "FR": "eur",
    "CA": "cad",
    "AU": "aud"
};

let baseAmount = 10.00;

const countrySelect = document.getElementById("country");
const currencyInput = document.getElementById("currency");
const amountDisplay = document.getElementById("amount-display");

function updateCurrencyAndAmount() {
    const currency = countryToCurrency[countrySelect.value];
    currencyInput.value = currency;

    let displayAmount = baseAmount;

    if (currency === "rsd") {
        displayAmount = (baseAmount * 100).toFixed(0);
        amountDisplay.textContent = `${displayAmount} RSD`;
    } else {
        displayAmount = baseAmount.toFixed(2);
        amountDisplay.textContent = `${displayAmount} ${currency.toUpperCase()}`;
    }
}

currencyInput.value = countryToCurrency[countrySelect.value];

countrySelect.addEventListener("change", () => {
    currencyInput.value = countryToCurrency[countrySelect.value];
});

countrySelect.addEventListener("change", updateCurrencyAndAmount);

updateCurrencyAndAmount();

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

    let amountToSend;
    if (currency === "rsd") {
        amountToSend = Math.round(baseAmount * 10000);
    } else {
        amountToSend = Math.round(baseAmount * 100);
    }
    
    const response = await fetch('/api/payments/create-payment-intent', {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({
            amount: amountToSend,
            currency: currencyInput.value
        })
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
