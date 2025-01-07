import 'package:chatworkmobile/utils/constants/sizes.dart';
import 'package:chatworkmobile/utils/constants/text_string.dart';
import 'package:chatworkmobile/utils/helper/helper_functions.dart';
import 'package:chatworkmobile/utils/images/image_strings.dart';
import 'package:flutter/material.dart';

class LoginHeader extends StatelessWidget {
  const LoginHeader({
    super.key,
  });

  @override
  Widget build(BuildContext context) {
    final isDark = THelperFunction.isDarkMode(context);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Image(
          height: 150,
          image: AssetImage(  isDark ? TImages.lightAppLogo: TImages.darkAppLogo)),
        Text(TText.loginTitle,style: Theme.of(context).textTheme.headlineMedium),
        const SizedBox(height: TSizes.fontSizeSm,),
        Text(TText.loginSubTitle, style: Theme.of(context).textTheme.bodyMedium,)
      ],
    );
  }
}