export type ActivityAction =
    | 'Create'
    | 'Update'
    | 'Delete'
    | 'StatusChange'
    | 'UploadMedia'
    | 'SetCoverImage'
    | 'DeleteMedia'
    | 'SelectMedia'
    | 'Publish';

export interface DashboardStatistics {
    total: number;
    created: number;
    pending: number;
    inReview: number;
    delivered: number;
}

export interface DashboardActivity {
    listingCaseId: number;
    listingCaseTitle: string;
    action: ActivityAction;
    message: string | null;
    timestamp: string;
}

export interface DashboardResponse {
    statistics: DashboardStatistics;
    latestActivities: DashboardActivity[];
}