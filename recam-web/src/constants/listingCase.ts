import type { ListcaseStatus, PropertyType, SaleCategory } from "../types/listingCase";

export const LISTCASE_STATUS_LABELS: Record<ListcaseStatus, string> = {
    1: 'Created',
    2: 'Pending',
    3: 'In Review',
    4: 'Delivered',
};

export const PROPERTY_TYPE_LABELS: Record<PropertyType, string> = {
    1: 'House',
    2: 'Unit',
    3: 'Townhouse',
    4: 'Villa',
    5: 'Others',
};

export const SALE_CATEGORY_LABELS: Record<SaleCategory, string> = {
    1: 'For Sale',
    2: 'For Rent',
    3: 'Auction',
};

export const LISTCASE_STATUS_STYLES: Record<ListcaseStatus, string> = {
    1: 'bg-gray-100 text-gray-700',
    2: 'bg-yellow-100 text-yellow-800',
    3: 'bg-blue-100 text-blue-800',
    4: 'bg-green-100 text-green-800',
};