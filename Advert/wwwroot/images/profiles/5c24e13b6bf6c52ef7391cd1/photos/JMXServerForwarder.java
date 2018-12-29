package router.jmx;

import org.apache.log4j.Logger;

import javax.management.*;
import javax.management.loading.ClassLoaderRepository;
import javax.management.remote.MBeanServerForwarder;
import java.io.ObjectInputStream;
import java.util.Iterator;
import java.util.Set;

public class JMXServerForwarder implements MBeanServerForwarder {
    private MBeanServer beanServer;

    private static Logger logger = Logger.getLogger(JMXServerForwarder.class);

    @Override
    public MBeanServer getMBeanServer() {
        logger.trace("public MBeanServer getMBeanServer()");
        return this.beanServer;
    }

    @Override
    public void setMBeanServer(MBeanServer beanServer) {
        logger.trace("public void setMBeanServer(MBeanServer beanServer)");
        this.beanServer = beanServer;
    }

    @Override
    public ObjectInstance createMBean(String className, ObjectName name) throws ReflectionException, InstanceAlreadyExistsException, MBeanException, NotCompliantMBeanException {
        logger.trace("public ObjectInstance createMBean(String className, ObjectName name)");
        return beanServer.createMBean(className, name);
    }

    @Override
    public ObjectInstance createMBean(String className, ObjectName name, ObjectName loaderName) throws ReflectionException, InstanceAlreadyExistsException, MBeanException, NotCompliantMBeanException, InstanceNotFoundException {
        logger.trace("public ObjectInstance createMBean(String className, ObjectName name, ObjectName loaderName)");
        return beanServer.createMBean(className, name, loaderName);
    }

    @Override
    public ObjectInstance createMBean(String className, ObjectName name, Object[] params, String[] signature) throws ReflectionException, InstanceAlreadyExistsException, MBeanException, NotCompliantMBeanException {
        logger.trace("public ObjectInstance createMBean(String className, ObjectName name, Object[] params, String[] signature)");
        return beanServer.createMBean(className, name, params, signature);
    }

    @Override
    public ObjectInstance createMBean(String className, ObjectName name, ObjectName loaderName, Object[] params, String[] signature) throws ReflectionException, InstanceAlreadyExistsException, MBeanException, NotCompliantMBeanException, InstanceNotFoundException {
        logger.trace("public ObjectInstance createMBean(String className, ObjectName name, Object[] params, String[] signature)");
        return beanServer.createMBean(className, name, loaderName, params, signature);
    }

    @Override
    public ObjectInstance registerMBean(Object object, ObjectName name) throws InstanceAlreadyExistsException, MBeanRegistrationException, NotCompliantMBeanException {
        logger.trace("public ObjectInstance registerMBean(Object object, ObjectName name)");
        return beanServer.registerMBean(object, name);
    }

    @Override
    public void unregisterMBean(ObjectName name) throws InstanceNotFoundException, MBeanRegistrationException {
        logger.trace("public void unregisterMBean(ObjectName name)");
        beanServer.unregisterMBean(name);
    }

    @Override
    public ObjectInstance getObjectInstance(ObjectName name) throws InstanceNotFoundException {
        logger.trace("public ObjectInstance getObjectInstance(ObjectName name)");
        return beanServer.getObjectInstance(name);
    }

    @Override
    public Set<ObjectInstance> queryMBeans(ObjectName name, QueryExp query) {
        logger.trace("public Set<ObjectInstance> queryMBeans(ObjectName name, QueryExp query)");
        return beanServer.queryMBeans(name, query);
    }

    @Override
    public Set<ObjectName> queryNames(ObjectName name, QueryExp query) {
        logger.trace("public Set<ObjectName> queryNames(ObjectName name, QueryExp query)");
        return beanServer.queryNames(name, query);
    }

    @Override
    public boolean isRegistered(ObjectName name) {
        logger.trace("public boolean isRegistered(ObjectName name)");
        return beanServer.isRegistered(name);
    }

    @Override
    public Integer getMBeanCount() {
        logger.trace("public Integer getMBeanCount()");
        return beanServer.getMBeanCount();
    }

    @Override
    public Object getAttribute(ObjectName name, String attribute) throws MBeanException, AttributeNotFoundException, InstanceNotFoundException, ReflectionException {
        logger.trace("public Object getAttribute(ObjectName name, String attribute)");
        return beanServer.getAttribute(name, attribute);
    }

    @Override
    public AttributeList getAttributes(ObjectName name, String[] attributes) throws InstanceNotFoundException, ReflectionException {
        logger.trace("public AttributeList getAttributes(ObjectName name, String[] attributes)");

        AttributeList attrList = beanServer.getAttributes(name, attributes);

        Iterator<Object> it = attrList.iterator();

        while (it.hasNext()) {
            logger.trace(it.next().toString());
        }

        return attrList;
    }

    @Override
    public void setAttribute(ObjectName name, Attribute attribute) throws InstanceNotFoundException, AttributeNotFoundException, InvalidAttributeValueException, MBeanException, ReflectionException {
        logger.trace("public void setAttribute(ObjectName name, Attribute attribute)");
        beanServer.setAttribute(name, attribute);
    }

    @Override
    public AttributeList setAttributes(ObjectName name, AttributeList attributes) throws InstanceNotFoundException, ReflectionException {
        logger.trace("public AttributeList setAttributes(ObjectName name, AttributeList attributes)");
        return beanServer.setAttributes(name, attributes);
    }

    @Override
    public Object invoke(ObjectName name, String operationName, Object[] params, String[] signature) throws InstanceNotFoundException, MBeanException, ReflectionException {
        logger.trace("public Object invoke(ObjectName name, String operationName, Object[] params, String[] signature)");
        return beanServer.invoke(name, operationName, params, signature);
    }

    @Override
    public String getDefaultDomain() {
        logger.trace("public String getDefaultDomain()");
        return beanServer.getDefaultDomain();
    }

    @Override
    public String[] getDomains() {
        logger.trace("public String[] getDomains()");
        return beanServer.getDomains();
    }

    @Override
    public void addNotificationListener(ObjectName name, NotificationListener listener, NotificationFilter filter, Object handback) throws InstanceNotFoundException {
        logger.trace("public void addNotificationListener(ObjectName name, NotificationListener listener, NotificationFilter filter, Object handback)");
        beanServer.addNotificationListener(name, listener, filter, handback);
    }

    @Override
    public void addNotificationListener(ObjectName name, ObjectName listener, NotificationFilter filter, Object handback) throws InstanceNotFoundException {
        logger.trace("public void addNotificationListener(ObjectName name, ObjectName listener, NotificationFilter filter, Object handback)");
        beanServer.addNotificationListener(name, listener, filter, handback);
    }

    @Override
    public void removeNotificationListener(ObjectName name, ObjectName listener) throws InstanceNotFoundException, ListenerNotFoundException {
        logger.trace("public void removeNotificationListener(ObjectName name, ObjectName listener)");
        beanServer.removeNotificationListener(name, listener);
    }

    @Override
    public void removeNotificationListener(ObjectName name, ObjectName listener, NotificationFilter filter, Object handback) throws InstanceNotFoundException, ListenerNotFoundException {
        logger.trace("public void removeNotificationListener(ObjectName name, ObjectName listener, NotificationFilter filter, Object handback)");
        beanServer.removeNotificationListener(name, listener, filter, handback);
    }

    @Override
    public void removeNotificationListener(ObjectName name, NotificationListener listener) throws InstanceNotFoundException, ListenerNotFoundException {
        logger.trace("public void removeNotificationListener(ObjectName name, NotificationListener listener)");
        beanServer.removeNotificationListener(name, listener);
    }

    @Override
    public void removeNotificationListener(ObjectName name, NotificationListener listener, NotificationFilter filter, Object handback) throws InstanceNotFoundException, ListenerNotFoundException {
        logger.trace("public void removeNotificationListener(ObjectName name, NotificationListener listener, NotificationFilter filter, Object handback)");
        beanServer.removeNotificationListener(name, listener, filter, handback);
    }

    @Override
    public MBeanInfo getMBeanInfo(ObjectName name) throws InstanceNotFoundException, IntrospectionException, ReflectionException {
        logger.trace("public MBeanInfo getMBeanInfo(ObjectName name)");
        return beanServer.getMBeanInfo(name);
    }

    @Override
    public boolean isInstanceOf(ObjectName name, String className) throws InstanceNotFoundException {
        logger.trace("public boolean isInstanceOf(ObjectName name, String className)");
        return beanServer.isInstanceOf(name, className);
    }

    @Override
    public Object instantiate(String className) throws ReflectionException, MBeanException {
        logger.trace("public Object instantiate(String className)");
        return beanServer.instantiate(className);
    }

    @Override
    public Object instantiate(String className, ObjectName loaderName) throws ReflectionException, MBeanException, InstanceNotFoundException {
        logger.trace("public Object instantiate(String className, ObjectName loaderName)");
        return beanServer.instantiate(className, loaderName);
    }

    @Override
    public Object instantiate(String className, Object[] params, String[] signature) throws ReflectionException, MBeanException {
        logger.trace("public Object instantiate(String className, Object[] params, String[] signature)");
        return beanServer.instantiate(className, params, signature);
    }

    @Override
    public Object instantiate(String className, ObjectName loaderName, Object[] params, String[] signature) throws ReflectionException, MBeanException, InstanceNotFoundException {
        logger.trace("public Object instantiate(String className, ObjectName loaderName, Object[] params, String[] signature)");
        return beanServer.instantiate(className, loaderName, params, signature);
    }

    @Override
    public ObjectInputStream deserialize(ObjectName name, byte[] data) throws InstanceNotFoundException, OperationsException {
        logger.trace("public ObjectInputStream deserialize(ObjectName name, byte[] data) ");
        return beanServer.deserialize(name, data);
    }

    @Override
    public ObjectInputStream deserialize(String className, byte[] data) throws OperationsException, ReflectionException {
        logger.trace("public ObjectInputStream deserialize(String className, byte[] data)");
        return beanServer.deserialize(className, data);
    }

    @Override
    public ObjectInputStream deserialize(String className, ObjectName loaderName, byte[] data) throws InstanceNotFoundException, OperationsException, ReflectionException {
        logger.trace("public ObjectInputStream deserialize(String className, ObjectName loaderName, byte[] data)");
        return beanServer.deserialize(className, loaderName, data);
    }

    @Override
    public ClassLoader getClassLoaderFor(ObjectName mbeanName) throws InstanceNotFoundException {
        logger.trace("public ClassLoader getClassLoaderFor(ObjectName mbeanName)");
        return beanServer.getClassLoader(mbeanName);
    }

    @Override
    public ClassLoader getClassLoader(ObjectName loaderName) throws InstanceNotFoundException {
        logger.trace("public ClassLoader getClassLoader(ObjectName loaderName)");
        return beanServer.getClassLoader(loaderName);
    }

    @Override
    public ClassLoaderRepository getClassLoaderRepository() {
        logger.trace("public ClassLoaderRepository getClassLoaderRepository()");
        return beanServer.getClassLoaderRepository();
    }
}
