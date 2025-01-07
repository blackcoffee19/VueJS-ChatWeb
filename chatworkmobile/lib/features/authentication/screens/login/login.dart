import 'package:chatworkmobile/common/styles/spacing_styles.dart';
import 'package:chatworkmobile/features/authentication/screens/login/widgets/login_divider.dart';
import 'package:chatworkmobile/features/authentication/screens/login/widgets/login_form.dart';
import 'package:chatworkmobile/features/authentication/screens/login/widgets/login_header.dart';
import 'package:chatworkmobile/features/authentication/screens/login/widgets/social_buttons.dart';
import 'package:chatworkmobile/utils/constants/sizes.dart';
import 'package:chatworkmobile/utils/constants/text_string.dart';
import 'package:flutter/material.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SingleChildScrollView(
        child: Padding(
          padding: TSpacingStyle.paddingWithAppBarHeight,
          child: Column(
            children: [
              LoginHeader(),
              //Form
              LoginForm(),
              // --- Divider
              LoginDivider(dividerText:  TText.signInWith),
              const SizedBox(
                height: TSizes.spaceBtwSections,
              ),
              /// Footer
              SocialButtons()
            ],
          ),
          
          )
      ),
    );
  }
}

