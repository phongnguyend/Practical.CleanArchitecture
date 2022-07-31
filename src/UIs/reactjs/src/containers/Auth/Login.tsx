import { useEffect } from "react";
import { useSelector } from "react-redux";

const Login = () => {
    const { authService } = useSelector((state: any) => state.auth);

    useEffect(() => {
        authService.login()
    }, []);

    return <div>Logging In ...</div>;
}

export default Login;
