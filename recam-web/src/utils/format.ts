export const formatPrice = (price: number): string =>
    price.toLocaleString('en-AU', {
        style: 'currency',
        currency: 'AUD',
        maximumFractionDigits: 0,
    });