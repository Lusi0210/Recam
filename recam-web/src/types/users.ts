import type { RoleName } from "./auth";

export interface User{
    id:string,
    email:string,
    role:RoleName,
    createdAt:string
}