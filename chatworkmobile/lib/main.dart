import 'package:chatworkmobile/features/authentication/controllers/auth_provider.dart';
import 'package:chatworkmobile/features/authentication/screens/login/login.dart';
import 'package:chatworkmobile/features/personalization/screens/home/home.dart';
import 'package:chatworkmobile/utils/theme/theme.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

void main() {
  runApp(
    MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (context) => AuthProvider())
      ],
      child: const MyApp(),
    ),
  );
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'ChatWork',
      themeMode: ThemeMode.system,
      theme: TAppTheme.lightTheme,
      darkTheme: TAppTheme.darkTheme,
      initialRoute: '/',
      routes: {
        '/': (context) => Consumer<AuthProvider>(
          builder: (context, auth, _) => auth.isAuthenticated ? HomeScreen(): LoginScreen(),
          ),
        '/home': (context)=> HomeScreen()
      },
    );
  }
}
