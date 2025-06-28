# ZenjectExtraBindInfo
Дополнительная инфа при "Unable to resolve". Показать в логе возможные типы, которые были забинжены неправильно (например, будет подсказка если использован Path&lt;Vector2Int> instead of Path&lt;int, int>)

Usage:
Вызвать статический метод
    ```ZenjectExtraBindInfo.CompareTypes( memberType, AllContracts );```
в DiContainer.cs, перед 
```
throw Assert.CreateException("Unable to resolve '{0}'{1}. Object graph:\n{2}", context.BindingId,
    (context.ObjectType == null ? "" : " while building object with type '{0}'".Fmt(context.ObjectType)),
```

todo
- нужно вызвать ещё раз ниже в DiContainer.cs?
- отрефакторить ZenjectExtraBindInfo.cs
- to github gist?
- to package?
- PR to Zenject repo? =)


