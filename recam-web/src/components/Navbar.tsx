import { useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";

interface NavbarProps {
    onMenuClick: () => void;
}

const Navbar = ({ onMenuClick }: NavbarProps) => {
    const { user, logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = (): void => {
        logout();
        navigate('/login', { replace: true });
    };

    return (
        <header className="h-16 bg-white border-b border-gray-200 px-4 md:px-6 flex justify-between items-center">
            <div className="flex items-center gap-3">
                <button
                    onClick={onMenuClick}
                    className="md:hidden text-2xl leading-none text-gray-700"
                    aria-label="Open menu"
                >
                    ☰
                </button>
                <span className="text-xl font-bold text-blue-600">RECAM</span>
            </div>

            <div className="flex items-center gap-4">
                <div className="text-right hidden sm:block">
                    <p className="text-sm font-medium text-gray-800">{user?.email}</p>
                    <p className="text-xs text-gray-500">{user?.role}</p>
                </div>
                <button
                    onClick={handleLogout}
                    className="text-sm px-3 py-1.5 rounded border border-gray-300 hover:bg-gray-100"
                >
                    Logout
                </button>
            </div>
        </header>
    );
};

export default Navbar;