import type { ReactNode } from "react";
import { ToastContext, type ToastType } from "./ToastContext";
import { useState, } from "react";

interface ToastProviderProps {
    children:ReactNode
}

interface Toast {
    id: string;
    message: string;
    type: ToastType;
}


const ToastProvider = ({children}:ToastProviderProps) => {
    const [toasts, setToasts] = useState<Toast[]>([]);

    const showToast = (message: string, type: ToastType): void => {
        const id: string = crypto.randomUUID();
        setToasts((prev) => [...prev, { id, message, type }]);

        setTimeout(() => {
            setToasts((prev) => prev.filter((toast) => toast.id !== id));
        }, 3000);
    };

    return (
        <ToastContext value={{ showToast }}>
            {children}
            <div className="fixed bottom-4 right-4 z-50 flex flex-col gap-2">
                {toasts.map((toast) => (
                    <div
                        key={toast.id}
                        className={`px-4 py-3 rounded shadow-lg text-sm text-white ${
                            toast.type === 'success' ? 'bg-green-600' : 'bg-red-600'
                        }`}
                    >
                        {toast.message}
                    </div>
                ))}
            </div>
        </ToastContext>
    );
}

export default ToastProvider;