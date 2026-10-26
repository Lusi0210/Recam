import { NavLink } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import type { RoleName } from "../types/auth";

interface MenuItem {
    label: string;
    path: string;
    roles?: RoleName[];
}

const menuItems: MenuItem[] = [
    { label: 'Listing Cases', path: '/' },
    { label: 'Users', path: '/users', roles: ['PhotographyCompany'] },
];

interface SidebarProps {
    isOpen: boolean;
    onClose: () => void;
}

const Sidebar = ({ isOpen, onClose }: SidebarProps) => {
    const { user } = useAuth();

    const visibleItems: MenuItem[] = menuItems.filter(
        (item) => !item.roles || (user !== null && item.roles.includes(user.role))
    );

    return (
        <>
            {isOpen && (
                <div
                    className="fixed inset-0 bg-black/30 z-30 md:hidden"
                    onClick={onClose}
                />
            )}
            <nav
                className={`fixed inset-y-0 left-0 z-40 w-56 bg-white border-r border-gray-200 p-4 flex flex-col gap-1
                    transition-transform md:static md:translate-x-0
                    ${isOpen ? 'translate-x-0' : '-translate-x-full'}`}
            >
                {visibleItems.map((item) => (
                    <NavLink
                        key={item.path}
                        to={item.path}
                        end
                        onClick={onClose}
                        className={({ isActive }) =>
                            `px-3 py-2 rounded text-sm ${
                                isActive
                                    ? 'bg-blue-50 text-blue-600 font-medium'
                                    : 'text-gray-700 hover:bg-gray-100'
                            }`
                        }
                    >
                        {item.label}
                    </NavLink>
                ))}
            </nav>
        </>
    );
};

export default Sidebar;