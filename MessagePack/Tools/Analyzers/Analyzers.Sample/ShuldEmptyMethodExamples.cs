// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;
using Analyzers.Utils;

namespace Analyzers.Sample;

public class VirtualDerived : Base {
    protected override void Foo_AvailableOverride() {
        Console.WriteLine("Do some staff");
    }
    
    protected override void Foo_HaveToOverride() {
        Console.WriteLine("Do some staff");
    }
}

public class DerivedFromVirtual : VirtualDerived {
    protected override void Foo_AvailableOverride() {
        base.Foo_AvailableOverride(); //no issue
    }

    protected override void Foo_HaveToOverride() { //has issue
        Console.WriteLine("Do another staff");
        
        base.Foo_AvailableOverride(); //do wrong base call as addition
    }

    protected override void Foo_HasAttributeEmptyBody() {
        base.Foo_HasAttributeEmptyBody(); //has issue
    }
}

public class Derived : Base {
    public void DerivedMethod_NoIssueForAllBaseMethods() {
        Foo_HasAttributeEmptyBody();
        FooReturningValue_HasIssue();
        Foo_ReturningValueHasBody_HasIssue();
        Foo_AvailableOverride();
        Foo_HaveToOverride();
        Foo_WithException_HasIssue();
        Foo_EmptyWithComment_NoIssue();
        Bar_WithBody_HasIssue();
        Baz_NoAttribute_NoIssue();
    }
    
    protected override void Foo_HasAttributeEmptyBody() {
        base.Foo_HasAttributeEmptyBody(); //has issue
    }

    protected override void Bar_WithBody_HasIssue() {
        base.Bar_WithBody_HasIssue(); //has issue
    }

    protected override void Baz_NoAttribute_NoIssue() {
        base.Baz_NoAttribute_NoIssue(); //no issue
    }
}

public abstract class Base {
    [ShouldBeEmpty]
    protected virtual void Foo_HasAttributeEmptyBody() { }

    [ShouldBeEmpty]
    protected virtual bool FooReturningValue_HasIssue() {
        return true;
    }

    [ShouldBeEmpty]
    protected virtual bool Foo_ReturningValueHasBody_HasIssue() {
        Console.WriteLine("Do some staff");
        return true;
    }

    [ShouldBeEmpty]
    protected virtual void Foo_AvailableOverride() { }
    
    [ShouldBeEmpty]
    protected virtual void Foo_HaveToOverride() { }

    [ShouldBeEmpty]
    protected virtual void Foo_WithException_HasIssue() {
        throw new NotImplementedException();
    }

    [ShouldBeEmpty]
    protected virtual void Foo_EmptyWithComment_NoIssue() {
        //do nothing
    }

    [ShouldBeEmpty]
    protected virtual void Bar_WithBody_HasIssue() {
        Console.WriteLine("Do some staff");
    }

    protected virtual void Baz_NoAttribute_NoIssue() {
        Console.WriteLine("Do some staff");
    }

    private void BaseMethodCalling_NoIssue() {
        Foo_HasAttributeEmptyBody();
    }
}