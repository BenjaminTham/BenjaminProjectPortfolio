/**
 * array of routes that are accessible to public
 * these routes do not require authentication
 * @type {string[]}
 */
export const publicRoutes: string[] = [
    "/",
    "/api/system-status",
    "/api/tray/in-used",
];

/**
 * array of routes that are used for authentication
 * these routes will redirect logged in users to /settings
 * @@type {string[]}
 */
export const authRoutes: string[] = [
    "/auth/login",
    "/auth/register"
];

/**
 * the prefix for api authentication routes
 * routes that start with this prefix are used for api authentication purposes
 */
// this path cant seem to be reached,,, recheck after
export const apiAuthPrefix = "/api/auth";


/**
 * default redirect path after logging in
 */
export const DEFAULT_LOGIN_REDIRECT = "/";