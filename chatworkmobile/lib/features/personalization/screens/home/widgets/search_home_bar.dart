
import 'package:chatworkmobile/utils/constants/color.dart';
import 'package:chatworkmobile/utils/constants/text_string.dart';
import 'package:flutter/material.dart';

class HomeSearchBar extends StatelessWidget {
  const HomeSearchBar({
    super.key,
    required TextEditingController search,
  }) : _search = search;

  final TextEditingController _search;

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Flexible(
          flex: 6,
          fit: FlexFit.tight,
          child:TextFormField(
            controller: _search,
            decoration: InputDecoration(
                prefixIcon: Icon(Icons.search),
                labelText: TText.searchChats
            ),
          ),
        ),
        Flexible(
            flex: 1,
            fit: FlexFit.tight,
            child: IconButton(onPressed: (){}, icon: Icon(Icons.add), color: TColors.primary,))
      ],
    );
  }
}