using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Zenject
{
/// <summary>
/// to do кидать 1 лог, а не разными.
/// </summary>
public static class ZenjectExtraBindInfo
{
    public static void CompareTypes( Type memberType, IEnumerable<BindingId> typeInContainer )
    {
        foreach ( BindingId bindingId in typeInContainer )
            CompareTypes( memberType, bindingId.Type );
    }

    // Pathfinder<ICell> and SomeService<ICell>
    public static void CompareTypes( Type memberType, Type typeInContainer )
    {
        if ( AreGenericDefinitionsEqual( memberType, typeInContainer ) ) //wrong. should not send notify if i has List<Cell> and List<Item> //но иначе не поймаю Pathfinder<Cell> Pathfinder<Vector2Int>
        {
            Debug.LogWarning( $"Found same generic type: {typeInContainer.GetShortTypeName()}" );
        }

        CheckGenericContainment( memberType, typeInContainer );
        CheckGenericContainment( typeInContainer, memberType );

        CheckArrayRelation( memberType, typeInContainer );
        CheckArrayRelation( typeInContainer, memberType );

        CheckInheritanceAndInterfaces( memberType, typeInContainer );
        CheckInheritanceAndInterfaces( typeInContainer, memberType );

        CheckPartialNameMatch( memberType, typeInContainer );
    }

    /// <summary>
    /// (typeof(List<int>), typeof(Dictionary<int, int>)); //false
    /// (typeof(Dictionary<float, GameObject>)), typeof(Dictionary<int, int>)); //true
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    static bool AreGenericDefinitionsEqual( Type a, Type b ) =>
        a.IsGenericType && b.IsGenericType
                        && a.GetGenericTypeDefinition() == b.GetGenericTypeDefinition();

    /// <summary>
    /// typeof(Dictionary<int, string>), typeof(int) //true
    /// </summary>
    static void CheckGenericContainment( Type outerType, Type innerType )
    {
        if ( !outerType.IsGenericType )
            return;

        foreach ( Type genericArg in outerType.GetGenericArguments() )
            if ( genericArg == innerType )
                Debug.LogWarning( $"Found generic containment: {outerType.GetShortTypeName()} contains {innerType.GetShortTypeName()}" );
    }

    static void CheckArrayRelation( Type arrayCandidate, Type elementCandidate )
    {
        if ( !arrayCandidate.IsArray )
            return;

        Type elementType = arrayCandidate.GetElementType();
        if ( elementType == elementCandidate )
            Debug.LogWarning( $"Found array relation: {arrayCandidate.GetShortTypeName()} of {elementCandidate.GetShortTypeName()}" );
    }

    static void CheckInheritanceAndInterfaces( Type childCandidate, Type parentCandidate )
    {
        if ( parentCandidate.IsInterface )
        {
            Type[] interfaces = childCandidate.GetInterfaces();
            if ( interfaces.Any( i => i == parentCandidate ) )
                Debug.LogWarning( $"Implements interface: {childCandidate.GetShortTypeName()} → {parentCandidate.GetShortTypeName()}" );
        }
        else if ( childCandidate.IsSubclassOf( parentCandidate ) )
            Debug.LogWarning( $"Inherits from: {childCandidate.GetShortTypeName()} → {parentCandidate.GetShortTypeName()}" );
    }

    static void CheckPartialNameMatch( Type a, Type b ) //Partial name match: AStarPathfinderWithoutStepCosts`1 ↔ AStarPathfinderWithoutStepCosts`1
    {
        string nameA = CleanTypeName( a.Name );
        string nameB = CleanTypeName( b.Name );

        if ( nameA.Contains( nameB ) || nameB.Contains( nameA ) )
            Debug.LogWarning( $"Partial name match: {a.GetShortTypeName()} ↔ {b.GetShortTypeName()}" );
    }

    /// <summary>
    /// Удаление распространённых префиксов и суффиксов
    /// </summary>
    static string CleanTypeName( string name ) =>
        name
            .Replace( "I", "" )
            .Replace( "Data", "" )
            .Replace( "Config", "" )
            .Replace( "Service", "" )
            .Replace( "Provider", "" )
            .Replace( "Manager", "" )
            .Replace( "Model", "" )
            .Replace( "View", "" )
            .Replace( "Controller", "" );

    static string GetShortTypeName(this Type type)
    {
        string ns = type.Namespace;
        // string firstNs = ns != null && ns.Contains(".") ? ns.Substring(0, ns.IndexOf('.')) : ns;
        //System.Collections.Generic.Dictionary firstNs: System
        string lastNs;
        if ( ns != null && ns.Contains( "." ) )
            lastNs = ns.Substring( ns.LastIndexOf( '.' ) + 1 );
        else
            lastNs = ns;
        return $"{lastNs}.{type.Name}";
    }
}
}
