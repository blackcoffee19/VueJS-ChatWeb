import 'dart:convert';

import 'package:chatworkmobile/utils/constants/url.dart';
import 'package:http/http.dart' as http;
import 'package:flutter/material.dart';
import 'package:logger/logger.dart';
class AuthProvider with ChangeNotifier {
  String? _token;
  bool _isAuthenticated = false;
  int? _userId;
  // Getter cho token và trạng thái đăng nhập
  String? get token => _token;
  bool get isAuthenticated => _isAuthenticated;
  int? get userId => _userId;
  
  //Hàm Login và lưu token
  Future<void> login (String username, String password) async{
    final logger = Logger();
    logger.i("${AppUrl.api}/Auth/login \nusername: $username\npassword: $password");
    try {
      // Thêm header với Content-Type là application/json
      final headers = {'Content-Type': 'application/json'};

      // Chuyển dữ liệu sang định dạng JSON
      final body = jsonEncode({'username': username, 'password': password});
      final response = await http.post(Uri.parse("${AppUrl.api}/Auth/login"),
          headers: headers,
          body: body);
      logger.i(response.body);
      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);
        _token = data['token'];
        _isAuthenticated = true;
        _userId = data['userId'];
        notifyListeners();
      }else{
        throw Exception("Đăng nhập thất bại");
      }
    } catch (e) {
      _token =null;
      _isAuthenticated =false;
      _userId = null;
      logger.e(e);
    }
  }

  //Hàm đăng xuất
  void logout(){
    _token = null;
    _isAuthenticated = false;
    _userId = null;
    notifyListeners();
  }

}