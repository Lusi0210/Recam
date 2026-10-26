const Footer = () => {
    return (
        <footer className="h-12 bg-white border-t border-gray-200 flex items-center justify-center text-xs text-gray-500">
            © {new Date().getFullYear()} RECAM
        </footer>
    );
};

export default Footer;