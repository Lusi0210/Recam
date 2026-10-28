export interface Agent{
    id:string,
    agentFirstName:string,
    agentLastName:string,
    avatarUrl: string | null,
    companyName?:string,
    email:string
}