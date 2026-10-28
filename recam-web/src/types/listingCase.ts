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
