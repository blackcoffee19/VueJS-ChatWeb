import 'package:chatworkmobile/common/styles/spacing_styles.dart';
import 'package:chatworkmobile/features/authentication/controllers/auth_provider.dart';
import 'package:chatworkmobile/features/personalization/screens/home/widgets/search_home_bar.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  void _logout(BuildContext context){
    Provider.of<AuthProvider>(context,listen: false).logout();
    Navigator.pushReplacementNamed(context,'/');
  }
  @override
  Widget build(BuildContext context) {
    final TextEditingController _search = TextEditingController();
    return Scaffold(
      body: SingleChildScrollView(
        child: Padding(
          padding: TSpacingStyle.paddingWithAppBarHeight,
          child: Column(
            children: [
              HomeSearchBar(search: _search),
              Text("Home page"),
              TextButton(onPressed: () => _logout(context), child: Text("Logout"))
            ],
          ),
        ),
      ),
    );
  }
}
