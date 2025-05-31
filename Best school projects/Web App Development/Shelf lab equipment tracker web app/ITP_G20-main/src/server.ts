import {createServer, IncomingMessage, ServerResponse} from 'http';
import {parse} from 'url';
import next from 'next';
import {startMQTTListener} from '@/domain/mqttControl';
import {runRFIDReportingData} from '@/domain/rfidControl';
import {getPowerLevelConfig, logStatusChange, retrieveSystemStatus, setPowerLevelConfig} from '@/domain/systemControl';
import {checkNumberOfUsers, createNewUser} from '@/domain/userControl';

const dev = process.env.NODE_ENV !== 'production';
const hostname = 'localhost';
const port = 3000;
// when using middleware `hostname` and `port` must be provided below
const app = next({dev, hostname, port});
const handle = app.getRequestHandler();

app.prepare().then(() => {
    createServer(async (req: IncomingMessage, res: ServerResponse) => {
        try {
            // Be sure to pass `true` as the second argument to `url.parse`.
            // This tells it to parse the query portion of the URL.
            const parsedUrl = parse(req.url || '', true);
            const {pathname, query} = parsedUrl;

            if (pathname === '/a') {
                await app.render(req, res, '/a', query);
            } else if (pathname === '/b') {
                await app.render(req, res, '/b', query);
            } else {
                await handle(req, res, parsedUrl);
            }
        } catch (err) {
            console.error('Error occurred handling', req.url, err);
            res.statusCode = 500;
            res.end('internal server error');
        }
    })
        .once('error', (err: Error) => {
            console.error(err);
            process.exit(1);
        })
        .listen(port, async () => {
            console.log(`> Ready on http://${hostname}:${port}`);
            const numberOfUsers = await checkNumberOfUsers();
            if (numberOfUsers === 0) {
                // Create a default user
                const email = process.env.DEFAULT_USER_EMAIL;
                const password = process.env.DEFAULT_USER_PASSWORD;
                console.log('Creating default user with email:', email);
                if (!email || !password) {
                    console.error('No default user email or password provided');
                    process.exit(1);
                }
                const {error} = await createNewUser({email, password: password, confirmPassword: password});
                if (error) {
                    console.error('Error creating default user:', error);
                    process.exit(1);
                }
            }

            // Check if the power level configuration is set and set it if not
            const powerLevelConfig = await getPowerLevelConfig();
            console.log(powerLevelConfig);
            if (!powerLevelConfig) {
                console.log('Setting default power level configuration');
                await setPowerLevelConfig({inventoryPower: 30, geofencingPower: 30, readwritePower: 15})
            }

            const status = await retrieveSystemStatus();
            if (status.systemStatus) {
                await logStatusChange(false);
            }
            setInterval(runRFIDReportingData, 1000);
            await startMQTTListener();
        });
});