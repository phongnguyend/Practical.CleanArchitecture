import { AuthService } from './auth.service';

export function AuthInitializer(auth: AuthService) { 
    return () => auth.loadUser(); 
};