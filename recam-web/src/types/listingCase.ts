
export interface ListingCase{
    id:number,
    title: string,
    street: string,
    city: string,
    state: string,
    price: number,
    listcaseStatus: ListcaseStatus,
    propertyType: PropertyType,
    saleCategory: SaleCategory
}

export interface ListingCaseQuery{
    pageNumber?: number,
    pageSize?: number,
    status?:ListcaseStatus,
    propertyType?:PropertyType,
    saleCategory?:SaleCategory,
    city?:string,
    minPrice?:number,
    maxPrice?:number,
    search?:string
    sortBy?: SortField,
    sortDirection?: SortDirection
}
export type ListcaseStatus = 1 | 2 | 3 | 4;
export type PropertyType = 1 | 2 | 3 | 4 | 5;
export type SaleCategory = 1 | 2 | 3;

export type SortField = 'title' | 'city' | 'price' | 'status'
export type SortDirection = 'asc' | 'desc';

export interface CreateListingCaseRequest{
    title:string,
    description?:string,
    street:string,
    city:string,
    state:string,
    postcode:string,
    longitude:number
    latitude:number
    price:number
    bedrooms:number,
    bathrooms:number,
    garages:number,
    floorArea:number
    propertyType:PropertyType
    saleCategory:SaleCategory
}

export interface CreateListingCaseResponse{
    id:number
}