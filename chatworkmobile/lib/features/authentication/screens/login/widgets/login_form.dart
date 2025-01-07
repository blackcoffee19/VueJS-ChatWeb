import 'package:chatworkmobile/features/authentication/controllers/auth_provider.dart';
import 'package:chatworkmobile/utils/constants/color.dart';
import 'package:chatworkmobile/utils/constants/sizes.dart';
import 'package:chatworkmobile/utils/constants/text_string.dart';
import 'package:flutter/material.dart';
import 'package:iconsax/iconsax.dart';
import 'package:provider/provider.dart';

class LoginForm extends StatefulWidget {
  const LoginForm({
    super.key,
  });

  @override
  State<LoginForm> createState() => _LoginFormState();
}

class _LoginFormState extends State<LoginForm> {
  final TextEditingController _username = TextEditingController();
  final TextEditingController _password = TextEditingController();
  final _formKey = GlobalKey<FormState>();
  bool _rememberMe = false;
  bool _isPasswordVisible =false;
  bool _isLoading = false;
  void _login(BuildContext context) async{

    if(_formKey.currentState!.validate()){
      setState(() {
        _isLoading = true;
      });
      final username = _username.text.trim();
      final password = _password.text;
      try {
        await Provider.of<AuthProvider>(context,listen: false).login(username, password);

        if (mounted) {
          if (Provider.of<AuthProvider>(context, listen: false).isAuthenticated) {
            Navigator.pushReplacementNamed(context, '/home');
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(
                  backgroundColor: TColors.success,
                  content: Text("Logged in successfully!", style: TextStyle(color: TColors.white, fontWeight: FontWeight.bold),)),
            );
          } else {
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(
                  backgroundColor: TColors.error,
                  content: Text("Log in failure", style: TextStyle(color: TColors.white, fontWeight: FontWeight.bold)),
              )
            );
          }
        }
      } catch (e) {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
                backgroundColor: TColors.error,
                content: Text("Log in failure", style: TextStyle(color: TColors.white, fontWeight: FontWeight.bold)),
          ));
        }
        Navigator.pushReplacementNamed(context,'/');
      }
      finally {
        if (mounted) {
          setState(() {
            _isLoading = false;
          });
        }
      }
    }
  }
  @override
  Widget build(BuildContext context) {
    return Form(
      key: _formKey,
      child: 
      Padding(
        padding: const EdgeInsets.symmetric(vertical: TSizes.spaceBtwSections
        ),
        child: Column(
        children: [
          TextFormField(
            controller: _username,
            decoration: InputDecoration(
              prefixIcon: Icon(Icons.person),
              labelText: TText.username,
            ),
            validator: (value){
              if(value==null|| value.isEmpty){
                return "Please input your username";
              }
              return null;
            },
          ),
          const SizedBox(height: TSizes.spaceBtwInputFields,),
          TextFormField(
            controller: _password,
            obscureText: !_isPasswordVisible, // Ẩn/Hiện mật khẩu
            decoration:  InputDecoration(
              prefixIcon: IconButton(
                onPressed: () {
                  setState(() {
                    _isPasswordVisible = !_isPasswordVisible;
                  });
                } , icon:  Icon(Iconsax.password_check)), 
              labelText: TText.password, 
              suffixIcon: Icon(Iconsax.eye_slash)
              ),
            validator: (value){
              if (value==null || value.isEmpty) {
                return "Please input your password";
              }
              return null;
            },
          ),
          const SizedBox(height: TSizes.spaceBtwInputFields/2),
        
          //Remember Me
          Row(children: [
            Row(
              children: [
                Checkbox(
                  value: _rememberMe, 
                  onChanged: (value){
                    setState(() {
                      _rememberMe = value??false;
                    });
                  }),
                const Text(TText.rememberMe)
              ],
            ),
            TextButton(onPressed: (){}, child: const Text(TText.forgetPassword))
          ],
          ),
          const SizedBox(height: TSizes.spaceBtwInputFields,),
          //Sign in Button
          SizedBox(
            width: double.infinity, 
            child: ElevatedButton(
              onPressed: () => _login(context), 
              child: const Text(TText.signIn)),),
          const SizedBox(height: TSizes.spaceBtwItems,),
          SizedBox(
            width: double.infinity,
            child: OutlinedButton(onPressed: (){}, 
            child: const Text(TText.createAccount)),
          )
        ],
      ),
      )
    );
  }
  @override
  void dispose() {
    _username.dispose();
    _password.dispose();
    super.dispose();
  }
}

