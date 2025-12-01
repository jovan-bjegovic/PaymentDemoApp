const countryToCurrency = {
    "RS": "EUR",
    "US": "USD",
    "GB": "GBP",
    "DE": "EUR",
    "FR": "EUR",
    "CA": "CAD",
    "AU": "AUD"
};

const baseAmount = 10.00;

const countrySelect = document.getElementById("country");
const currencyInput = document.getElementById("currency");
const amountDisplay = document.getElementById("amount-display");
const payButton = document.getElementById("submit-token");

const tokenizerContainer = "div-hosted-tokenization";

let tokenizer = null;
let hostedUrl = null;
let isProcessing = false;

function setProcessing(value)
{
    isProcessing = value;
    if (payButton)
    {
        payButton.disabled = value;
    }
}

function updateCurrencyAndAmount()
{
    if (!countrySelect || !currencyInput || !amountDisplay)
    {
        return;
    }

    const currency = countryToCurrency[countrySelect.value] || "EUR";
    currencyInput.value = currency;

    amountDisplay.textContent = baseAmount.toFixed(2) + " " + currency;
}

async function initTokenizer()
{
    setProcessing(true);

    try
    {
        const response = await fetch("/api/payments/create-token", { method: "POST" });

        if (!response.ok)
        {
            return;
        }

        const data = await response.json();
        hostedUrl = data.token || data.url || null;

        if (!hostedUrl || typeof Tokenizer === "undefined")
        {
            return;
        }

        tokenizer = new Tokenizer(hostedUrl, tokenizerContainer, { hideCardholderName: false });
        await tokenizer.initialize();
    }
    catch
    {
    }
    finally
    {
        setProcessing(false);
    }
}

async function onPayClicked(event)
{
    event.preventDefault();

    if (isProcessing)
    {
        return;
    }

    setProcessing(true);

    try
    {
        const name = (document.getElementById("name").value || "").trim();
        const email = (document.getElementById("email").value || "").trim();
        const country = (countrySelect.value || "").trim();
        const currency = (currencyInput.value || "").trim().toLowerCase();

        if (!name || !email || !tokenizer)
        {
            window.location.href = "/checkout-cancel";
            return;
        }

        const tokenResult = await tokenizer.submitTokenization();
        const hostedTokenizationId = tokenResult?.hostedTokenizationId;

        if (!hostedTokenizationId)
        {
            window.location.href = "/checkout-cancel";
            return;
        }

        const payload = {
            amount: Math.round(baseAmount * 100),
            currency: currency,
            name: name,
            email: email,
            country: country,
            token: hostedTokenizationId
        };

        const payResponse = await fetch("/api/payments/create-payment", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        const payJson = await payResponse.json();

        if (payJson.success) {
            window.location.href = "/checkout-success";
        } else {
            window.location.href = "/checkout-cancel";
        }

    }
    catch
    {
        window.location.href = "/checkout-cancel";
    }
    finally
    {
        setProcessing(false);
    }
}

document.addEventListener("DOMContentLoaded", function()
{
    updateCurrencyAndAmount();
    initTokenizer();

    if (countrySelect)
    {
        countrySelect.addEventListener("change", updateCurrencyAndAmount);
    }

    if (payButton)
    {
        payButton.addEventListener("click", onPayClicked);
    }
});
