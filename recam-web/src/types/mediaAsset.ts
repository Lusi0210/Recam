
export type MediaType = 1 | 2 | 3 | 4;

export interface MediaAsset{
    id:number,
    mediaType:MediaType,
    mediaUrl:string,
    uploadedAt:string,
    userId:string,
    isHero:boolean,
    isSelected:boolean,

}