import { useState } from "react";
import { Outlet } from "react-router-dom";
import Navbar from "../components/Navbar";
import Sidebar from "../components/Sidebar";
import Footer from "../components/Footer";

const MainLayout = () => {
    const [isSidebarOpen, setIsSidebarOpen] = useState<boolean>(false);

    return (
        <div className="min-h-screen flex flex-col bg-gray-50">
            <Navbar onMenuClick={() => setIsSidebarOpen(true)} />
            <div className="flex flex-1">
                <Sidebar isOpen={isSidebarOpen} onClose={() => setIsSidebarOpen(false)} />
                <main className="flex-1 p-4 md:p-6">
                    <Outlet />
                </main>
            </div>
            <Footer />
        </div>
    );
};

export default MainLayout;