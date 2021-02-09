import prod from './environment';
import dev from './environment.dev';

const env = process.env.NODE_ENV == 'development' ? dev : prod;
export default env;