import { useContext } from 'react';
import { ToastContext, type ToastContextType } from '../contexts/ToastContext';

export const useToast = (): ToastContextType => {
    const context: ToastContextType | null = useContext(ToastContext);
    if (!context) {
        throw new Error('useToast must be used within ToastProvider');
    }
    return context;
};