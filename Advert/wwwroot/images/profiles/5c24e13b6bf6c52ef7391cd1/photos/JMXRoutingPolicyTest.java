package router;

import org.apache.log4j.Logger;
import org.junit.Test;
import sun.management.jmxremote.ConnectorBootstrap;

import javax.management.remote.JMXConnectorServer;
import java.io.IOException;
import java.net.UnknownHostException;
import java.util.Properties;
import java.util.concurrent.CountDownLatch;

//172.19.0.4

public class JMXRoutingPolicyTest {
    private static Logger logger = Logger.getLogger(JMXRoutingPolicyTest.class);

    @Test
    public void test() throws Exception {
        CountDownLatch shutdownLatch = new CountDownLatch(1);

        Properties jmxProps = new Properties();

        jmxProps.setProperty("com.sun.management.jmxremote.port", "9011");
        jmxProps.setProperty("com.sun.management.jmxremote.rmi.port", "9011");
        jmxProps.setProperty("com.sun.management.jmxremote.ssl", "false");
        jmxProps.setProperty("com.sun.management.jmxremote.authenticate", "false");

        JMXConnectorServer jmxRemoteConnectorServer = ConnectorBootstrap.startRemoteConnectorServer("9011", jmxProps);

//        ConnectorServer connectorServer = new ConnectorServer(jmxRemoteConnectorServer);

//        connectorServer.start();

        shutdownLatch.await();
    }

    @Test(expected = UnknownHostException.class)
    public void testConnectorServerForUnknownHostname() throws IOException {
//        String serviceUrl = new StringBuilder().append("service:jmx:jmxmp://").append("unknownhost").append(":").append(9011).toString();
//        JMXRouter orchestratorRouter = JMXRoutingPolicyBuilder.newBuilder().from(serviceUrl).toLocal(serviceUrl).build();

//        ConnectorServer orchestratorConnectorServer = orchestratorRouter.getConnectorById(serviceUrl);
//        Assert.assertNotNull(orchestratorConnectorServer);
//        orchestratorConnectorServer.start();
    }
}

/*
        MBeanServer beanServer = ManagementFactory.getPlatformMBeanServer();

        ObjectName objectName = ObjectNameBuilder.builder().setAttribute("type", "Deployments").setPackageName(JMXRoutingPolicyTest.class.getPackage().getName()).build();

        if (!beanServer.isRegistered(objectName)) {
            DeploymentMetrics deploymentMetrics = new DeploymentMetrics();
            deploymentMetrics.setDeploymentId(UUID.randomUUID().toString());
            beanServer.registerMBean(deploymentMetrics, objectName);
        }
 */
