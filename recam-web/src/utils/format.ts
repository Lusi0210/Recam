export const formatPrice = (price: number): string =>
    price.toLocaleString('en-AU', {
        style: 'currency',
        currency: 'AUD',
        maximumFractionDigits: 0,
    });

export const formatDateTime = (value: string): string =>
    new Date(value).toLocaleString('en-AU', {
        dateStyle: 'medium',
        timeStyle: 'short',
    });
