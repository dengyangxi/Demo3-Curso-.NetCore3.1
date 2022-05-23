apiVersion: v1
kind: Namespace
metadata:
  name: {{NameSpaceName}}-ns


---
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: statestore
  namespace: {{NameSpaceName}}-ns
spec:
  type: state.redis
  version: v1
  metadata:
  - name: redisHost
    value: 192.168.10.129:6379
  - name: redisPassword
    value: ""
  - name: actorStateStore
    value: "true"

---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: {{ServiceName}}
  namespace: {{NameSpaceName}}-ns
  labels:
    app: {{ServiceName}}
spec:
  replicas: 1
  selector:
    matchLabels:
      app: {{ServiceName}}
  template:
    metadata:
      namespace: {{NameSpaceName}}-ns
      labels:
        app: {{ServiceName}}
      annotations:
        dapr.io/enabled: "true"
        dapr.io/app-id: "{{ServiceName}}"
        dapr.io/app-port: "{{ServicePort}}"
    spec:
      imagePullSecrets:
      - name: harbor-key
      containers:
      - name: {{ServiceName}}
        image: 192.168.10.129/dapr/{{ServiceName}}:{{tagversion}}
        ports:
        - containerPort: {{ServicePort}}
        imagePullPolicy: Always
        # 容器监控检查  http/tcp 
        livenessProbe:
          #通过http方式检查监控状态
          httpGet:
               #检查方法
               path: /Hotel/Index
               #检查端口
               port: {{ServicePort}}
---
apiVersion: v1
kind: Service
metadata:
  namespace: {{NameSpaceName}}-ns
  name: {{ServiceName}}-svc
spec:
  type: NodePort
  selector:
    app: {{ServiceName}}
  ports:
    - port: {{ServicePort}}
      targetPort: {{ServicePort}}
      nodePort: {{NodePort}}
