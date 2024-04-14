Hướng dẫn cài đặt:

- Tải phiên bản Common Asset Clean mới nhất về và import vào dự án
- Copy đoạn code sau vào Packages/manifest.json (dán vào trong đoạn dependencies), Unity sẽ tự cài thư viện Addressable và In App Purchasing

{
  "dependencies": {
    ...
    "com.unity.addressables": "1.21.20",
    "com.unity.purchasing": "4.11.0",
    ...
  }
}
