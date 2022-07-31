import { useEffect } from "react";
import { useSelector } from "react-redux";

const Logout = () => {
    const { authService } = useSelector((state: any) => state.auth);

    useEffect(() => {
        authService.logout()
    }, []);

    return <div>Logging Out ...</div>;
}

export default Logout;

