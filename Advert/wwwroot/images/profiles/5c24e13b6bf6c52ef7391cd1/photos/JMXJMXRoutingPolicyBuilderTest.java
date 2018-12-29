package router;

import org.junit.Test;
import router.jmx.JMXRoutingPolicyBuilder;

import java.io.IOException;
import java.net.MalformedURLException;

public class JMXJMXRoutingPolicyBuilderTest {
    private final String LOCALHOST = "localhost";
    private final Integer JMX_PORT = 9010;

    @Test
    public void testValidServiceUrl() throws IOException {
        String serviceUrl = new StringBuilder().append("service:jmx:jmxmp://").append(LOCALHOST).append(":").append(JMX_PORT).toString();
        JMXRoutingPolicyBuilder.newBuilder().setUrls(serviceUrl).build();
    }

    @Test
    public void testValidServiceUrls() throws IOException {
        String serviceUrl1 = new StringBuilder().append("service:jmx:jmxmp://").append(LOCALHOST).append(":").append(JMX_PORT).toString();
        String serviceUrl2 = new StringBuilder().append("service:jmx:jmxmp://").append(LOCALHOST).append(":").append(JMX_PORT).toString();
        JMXRoutingPolicyBuilder.newBuilder().setUrls(serviceUrl1, serviceUrl2).build();
    }

    @Test(expected = MalformedURLException.class)
    public void testServiceUrlForNotValidHost() throws IOException {
        String serviceUrl = new StringBuilder().append("service:jmx:jmxmp://").append("!@#$%^&*()").append(":").append(JMX_PORT).toString();
        JMXRoutingPolicyBuilder.newBuilder().setUrls(serviceUrl).build();
    }

    @Test(expected = MalformedURLException.class)
    public void testServiceUrlForNotValidIpAddress() throws IOException {
        String serviceUrl = new StringBuilder().append("service:jmx:jmxmp://").append("512.512.512.512").append(":").append(JMX_PORT).toString();
        JMXRoutingPolicyBuilder.newBuilder().setUrls(serviceUrl).build();
    }
}
