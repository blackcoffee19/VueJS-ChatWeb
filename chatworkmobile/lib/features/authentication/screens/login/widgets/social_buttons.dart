import 'package:chatworkmobile/utils/constants/color.dart';
import 'package:chatworkmobile/utils/constants/sizes.dart';
import 'package:chatworkmobile/utils/helper/helper_functions.dart';
import 'package:chatworkmobile/utils/images/image_strings.dart';
import 'package:flutter/material.dart';

class SocialButtons extends StatelessWidget {
  const SocialButtons({
    super.key,
  });

  @override
  Widget build(BuildContext context) {
    final isDark = THelperFunction.isDarkMode(context);
    return Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Container(
          decoration: BoxDecoration(
             color: isDark? TColors.white: TColors.dark,
            border: Border.all(color: TColors.grey,),
            borderRadius: BorderRadius.circular(100)
          ),
          child: IconButton(
            onPressed: (){}, 
            icon: const Image(
              width: TSizes.iconMd,
              height: TSizes.iconMd,
              image: AssetImage(TImages.google))
              ),
        ),
        const SizedBox(width: TSizes.spaceBtwItems,),
        Container(
          decoration: BoxDecoration(
            color: isDark? TColors.white: TColors.dark,
            border: Border.all(color: TColors.grey,),
            borderRadius: BorderRadius.circular(100)
          ),
          child: IconButton(
            onPressed: (){}, 
            icon: const Image(
              width: TSizes.iconMd,
              height: TSizes.iconMd,
              image: AssetImage(TImages.facebook))
              ),
        ),
        const SizedBox(width: TSizes.spaceBtwItems,),
        Container(
          decoration: BoxDecoration(
            color: isDark? TColors.white: TColors.dark,
            border: Border.all(color: TColors.grey,),
            borderRadius: BorderRadius.circular(100)
          ),
          child: IconButton(
            onPressed: (){}, 
            icon: const Image(
              width: TSizes.iconMd,
              height: TSizes.iconMd,
              image: AssetImage(TImages.xTwitter))
              ),
        )
      ],
    );
  }
}

