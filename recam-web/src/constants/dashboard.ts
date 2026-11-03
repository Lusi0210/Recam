import type { ActivityAction } from "../types/dashboard";

export const ACTIVITY_ACTION_LABELS: Record<ActivityAction, string> = {
    Create: 'Created listing',
    Update: 'Updated details',
    Delete: 'Deleted listing',
    StatusChange: 'Changed status',
    UploadMedia: 'Uploaded media',
    SetCoverImage: 'Set cover image',
    DeleteMedia: 'Deleted media',
    SelectMedia: 'Selected media',
    Publish: 'Published listing',
};