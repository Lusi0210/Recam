import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getDashboard } from "../api/dashboardApi";
import type { DashboardResponse, DashboardStatistics } from "../types/dashboard";
import type { RoleName } from "../types/auth";
import { ACTIVITY_ACTION_LABELS } from "../constants/dashboard";
import { useAuth } from "../hooks/useAuth";
import { getErrorMessage } from "../utils/apiError";
import { formatDateTime } from "../utils/format";
import Skeleton from "../components/Skeleton";

interface StatCard {
    label: string;
    key: keyof DashboardStatistics;
    style: string;
}

const STAT_CARDS: StatCard[] = [
    { label: 'Total', key: 'total', style: 'text-gray-900' },
    { label: 'Created', key: 'created', style: 'text-gray-600' },
    { label: 'Pending', key: 'pending', style: 'text-yellow-600' },
    { label: 'In Review', key: 'inReview', style: 'text-blue-600' },
    { label: 'Delivered', key: 'delivered', style: 'text-green-600' },
];

interface QuickLink {
    title: string;
    description: string;
    path: string;
    roles?: RoleName[];
}

const QUICK_LINKS: QuickLink[] = [
    { title: 'Listing Cases', description: 'Browse, filter and manage listing cases', path: '/listing-cases' },
    { title: 'New Listing Case', description: 'Create a new property listing', path: '/listing-cases/new', roles: ['PhotographyCompany'] },
    { title: 'User Management', description: 'View all users in the system', path: '/users', roles: ['PhotographyCompany'] },
];

export default function DashboardPage() {
    const { user } = useAuth();
    const [dashboard, setDashboard] = useState<DashboardResponse | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [errorMessage, setErrorMessage] = useState<string>('');

    useEffect(() => {
        const fetchData = async (): Promise<void> => {
            setIsLoading(true);
            setErrorMessage('');
            try {
                const data: DashboardResponse = await getDashboard();
                setDashboard(data);
            } catch (error: unknown) {
                setErrorMessage(getErrorMessage(error, 'Failed to load dashboard'));
            } finally {
                setIsLoading(false);
            }
        };
        fetchData();
    }, []);

    const visibleLinks: QuickLink[] = QUICK_LINKS.filter(
        (link) => !link.roles || (user !== null && link.roles.includes(user.role))
    );

    return (
        <div className="flex flex-col gap-6">
            <div>
                <h1 className="text-2xl font-bold">Dashboard</h1>
                <p className="text-sm text-gray-500 mt-1">Welcome back, {user?.email}</p>
            </div>

            {errorMessage && (
                <p className="text-sm text-red-600 bg-red-50 p-3 rounded">{errorMessage}</p>
            )}

            <section className="grid grid-cols-2 md:grid-cols-5 gap-4">
                {STAT_CARDS.map((card) => (
                    <div key={card.key} className="bg-white rounded-lg shadow p-4">
                        <p className="text-sm text-gray-500">{card.label}</p>
                        {isLoading ? (
                            <Skeleton className="h-8 w-12 mt-2" />
                        ) : (
                            <p className={`text-3xl font-bold mt-1 ${card.style}`}>
                                {dashboard?.statistics[card.key] ?? 0}
                            </p>
                        )}
                    </div>
                ))}
            </section>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                <section className="lg:col-span-2 bg-white rounded-lg shadow">
                    <h2 className="px-4 py-3 border-b border-gray-100 font-semibold">Latest Activities</h2>
                    {isLoading ? (
                        <div className="p-4 flex flex-col gap-3">
                            {[1, 2, 3, 4].map((n) => (
                                <Skeleton key={n} className="h-10 w-full" />
                            ))}
                        </div>
                    ) : !dashboard || dashboard.latestActivities.length === 0 ? (
                        <p className="px-4 py-8 text-center text-sm text-gray-500">No activity yet</p>
                    ) : (
                        <ul className="divide-y divide-gray-100">
                            {dashboard.latestActivities.map((activity) => (
                                <li
                                    key={`${activity.listingCaseId}-${activity.timestamp}`}
                                    className="px-4 py-3 flex flex-wrap items-start justify-between gap-2"
                                >
                                    <div className="min-w-0">
                                        <p className="text-sm">
                                            <span className="font-medium text-gray-900">
                                                {ACTIVITY_ACTION_LABELS[activity.action]}
                                            </span>
                                            {' · '}
                                            <Link
                                                to={`/listing-cases/${activity.listingCaseId}`}
                                                className="text-blue-600 hover:underline"
                                            >
                                                {activity.listingCaseTitle}
                                            </Link>
                                        </p>
                                        {activity.message && (
                                            <p className="text-xs text-gray-500 mt-0.5">{activity.message}</p>
                                        )}
                                    </div>
                                    <time dateTime={activity.timestamp} className="text-xs text-gray-400 whitespace-nowrap">
                                        {formatDateTime(activity.timestamp)}
                                    </time>
                                </li>
                            ))}
                        </ul>
                    )}
                </section>

                <section className="flex flex-col gap-3">
                    <h2 className="font-semibold">Quick Navigation</h2>
                    {visibleLinks.map((link) => (
                        <Link
                            key={link.path}
                            to={link.path}
                            className="block bg-white rounded-lg shadow p-4 hover:shadow-md hover:ring-1 hover:ring-blue-200 transition"
                        >
                            <p className="font-medium text-blue-600">{link.title} →</p>
                            <p className="text-sm text-gray-500 mt-1">{link.description}</p>
                        </Link>
                    ))}
                </section>
            </div>
        </div>
    );
}