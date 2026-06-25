欢迎来到ma语言的世界！

你可以在解释器中输入
:show
以切换至显示过程模式。在该模式下，函数解释的过程会被逐行打印。
输入
:execute
切换至显示结果模式。在该模式下，当函数无法再解释时就会打印。这也是默认的模式。
输入
:load /path
可以读取文档里的函数。/path需要是文档的地址。
输入
:pause
切换至暂停模式。在该模式下，函数会解释一步，直至按下回车才会继续解释下一步。

以下是ma语言的介绍！

ma语言的程序是由函数组成的。
函数由2部分组成，名字和参数。
例如f(x,y)，对于f这个函数而言，f是名字，x和y是参数。

函数可以分成没有参数的函数，和有参数的函数。
没有参数的函数就是只有函数名的函数，例如f(x)里面的x。
有参数的函数也包括0个参数的函数，例如f()。

函数可以分成有定义的函数，和没有定义的函数。
有定义的函数包括系统自带的函数，还有使用def、defn函数定义的函数。
除此以外的都是没有定义的函数。

函数可以被解释，或是执行。
当一个函数被解释，就是根据这个函数的名字和参数，把这个函数的名字和参数进行改变，例如+(1,1)在解释后会变为2。
这个改变的结果被称为这个函数的返回值。函数返回x，指函数的返回值为x。
没有定义的函数，和没有参数的函数是不可解释的。
当一个函数被执行，对于一般执行顺序的函数来说，就是先执行自身的参数，直到自身的参数都是不可解释的函数时，解释自身。
一些函数有特定的执行顺序。
不可解释的函数的参数不会被执行。

系统函数包括：

call(name,actargs...)
把name的参数改为actargs，返回name。
actargs的数量可以是0到多个。
当name的名字为lam时，如call(lam(lamargs...,lambody),actargs...)，会将lambody中与lamargs同名的函数替换为与lamargs在顺序上对应的actargs，返回lambody。详见替换。
例子：
call(hello,world) => hello(world)
call(lam(x,+(x,1)),2) => +(2,1) => 3

if(cond,then,else)
当cond为True时，返回then。当cond为False时，返回else。
cond需要是True或False，若非如此，返回err。
存在特殊的执行顺序，先执行cond，然后解释自身。
例子：
if(>(2,1),big,small) => if(True,big,small) => big

def(name,args...,body)
定义一个函数，返回name。
args的数量可以是0到多个。
存在特殊的执行顺序，直接解释自身。
例子：
def(hello,x,world(x)) => hello ; hello(ma) => world(ma)

defn(name,args...,body)
定义一个函数，返回name。被定义的函数将会存在特殊的执行顺序，直接解释自身。
args的数量可以是0到多个。
存在特殊的执行顺序，直接解释自身。
例子：
defn(myif,cond,then,else,if(cond,then,else)) => myif ;
myif(>(2,1),+(1,1),/(1,0)) => if(>(2,1),+(1,1),/(1,0)) => if(True,+(1,1),/(1,0)) => +(1,1) => 2

当def、defn被解释后，可以使用name(actargs...)执行被定义的函数。
当被定义的函数解释时，会将body中的每个与args同名的函数替换为与args在顺序上对应的actargs，返回body。详见替换。

+、-、*、/(a,b)
加减乘除四则运算，将a和b的名字视作整数，进行整数的运算，返回运算的结果。
a和b的名字需要是整数。若a或b为非整数，或在/(a,b)中b为0，会返回err。
例子：
+(1,1) => 2

>、<(a,b)
大于和小于，将a和b的名字视作整数，进行整数的大小比较，若为真，返回True，否则返回False。
a和b的名字需要是整数。若a或b为非整数，会返回err。
例子：
>(2,1) => True

=、!=(a,b)
等于和不等于，对a和b的名字进行比较，若为真，返回True，否则返回False。
例子：
=(some,some) => True

len(x)
测量x的参数数量，返回该值。
若x为没有参数的函数，值为None。
例子：
len(list(a,b,c)) => 3
len(list()) => 0
len(list) => None

at(x,index)
取出x中的第index个参数，返回x。
index从0开始，可以使用负数的index取出x的倒数第index个参数。
x不能是没有参数的函数，或参数数量为0。index的名字需要是整数。index大小需要在[-x的参数数量,x的参数数量)中。否则返回err。
例子：
at(list(a,b,c),0) => a
at(list(a,b,c),-1) => c

atlist(x,index)
取出x中的第index个参数，返回以list为名字，以x中的第index个参数的名字，和该参数的参数为参数的函数。
设x的第index个参数为xname(arg1,arg2...)，那么就会返回list(xname,arg1,arg2...)。
index从0开始，可以使用负数的index取出x的倒数第index个参数。
x不能是没有参数的函数，或参数数量为0。index的名字需要是整数。index大小需要在[-x的参数数量,x的参数数量)中。否则会变为err。
例子：
atlist(list(+(1,2)),0) => list(+,1,2)

first(x)
取出x的第一个参数，返回那个参数。
若x没有参数，返回err。
例子：
first(list(1,2,3)) => 1

last(x)
取出x的最后一个参数，返回那个参数。
若x没有参数，返回err。
例子：
last(list(1,2,3)) => 3

append(x,item...)
把item添加至x的参数的最后一位，返回x。
item的数量可以是1个到多个。
若x是没有参数的函数，则会把x变成有1个参数的函数，参数为item。
例子：
append(list(1,2),3) => list(1,2,3)
append(list(a),b,c) => list(a,b,c)

insert(item,x)
把item添加至x的参数的第一位，返回x。
若x是没有参数的函数，则会把x变成有1个参数的函数，参数为item。
例子：
insert(1,list(2,3)) => list(1,2,3)

remove(x,index)
把x的第index个参数删掉，返回x。
index从0开始，可以使用负数的index删掉x的倒数第index个参数。
x不能是没有参数的函数，或参数数量为0。index的名字需要是整数。index大小需要在[-x的参数数量,x的参数数量)中。否则返回err。
例子：
remove(list(a,b,c),1) => list(a,c)

rest(x)
把x的第一个参数删掉，返回x。
若x没有参数，返回err。
例子：
rest(list(1,2,3)) => list(2,3)

pop(x)
把x的最后一个参数删掉，返回x。
若x没有参数，返回err。
例子：
pop(list(1,2,3)) => list(1,2)

set(x,index,item)
把x的第index个参数变为为item，返回x。
index从0开始，可以使用负数的index取x的倒数第index个参数。
x不能是没有参数的函数，或参数数量为0。index的名字需要是整数。index大小需要在[-x的参数数量,x的参数数量)中。否则会变为err。
例子：
set(list(a,b,c),0,d) => list(d,b,c)

rename(body,name)
把body的名字改为name的名字，返回body。
例子：
rename(hello(alice),bye) => bye(alice)

lookup(x,target)
在x的参数里查找和target同名的函数，返回那个函数。
若x没有参数，或没找到，返回err。
例子：
lookup(person(age(20),gender(male)),age) => age(20)

eq(x,y)
递归地查看x和y的名字，及内部是否完全相同，返回True或False。
内部指参数，以及参数的参数，等等。
存在特殊的执行顺序，直接解释自身。
例子：
eq(list(a,b(c)),list(a,b(c))) => True

rp(args...,target,struct,body)
对body内从左到右，从浅到深的第一个匹配上target的函数进行替换，替换为struct，返回body。
args的数量可以是0到多个。
target和struct内的作为函数的参数的和args同名的函数会被视为通配符，可以匹配任意的函数，并在替换时替换为对应的函数。
除args外名字、参数及参数的参数等与target相同的函数会被视作匹配，args会被替换为与被匹配函数中对应位置的函数，然后将struct中的args替换为args对应的函数，再将body中被匹配的函数替换为struct。
存在特殊的执行顺序，先执行body，再解释自身。
详见替换。
例子：
rp(x,f(x),g(x),list(f(a),f(b))) => list(g(a),f(b))

rpall(args...,target,struct,body)
对body内每一个匹配上target的函数进行替换，替换为struct，返回body。
存在特殊的执行顺序，先执行body，再解释自身。
其余见rp。
例子：
rpall(x,f(x),g(x),list(f(a),f(b))) => list(g(a),g(b))

wait(x)
接管x的执行。x不会进行执行，除了x内的以下几种函数。当x内不包含以下几种函数时，返回x。否则，返回wait(x)。
exp(x)
对x进行解释，返回x。
exe(x)
对x进行执行，直到x为不可解释的函数，返回x。（实现为若发生1次解释，返回exe(x)，若x不可解释，返回x。）
step(x)
对x进行执行，若发生1次解释或x不可解释，返回x。
expif(cond,then,else)
只要当cond为True时，返回then。只要当cond为False时，返回else。
其中exp、exe、step会按照从左到右，由深至浅的顺序进行解释。expif按照从左到右，由浅至深的顺序进行解释。
不会主动对x中的wait中的以上几种函数进行解释，若x包含wait且wait外无以上几种函数，返回x。若要对x内的wait进行解释，可以把x中的wait放入exp等上述函数中。
存在特殊的执行顺序，直接解释自身。
例子：
wait(exp(at(+(1,1),0))) => wait(1) => 1
wait(wait(exp(+(1,1)))) => wait(exp(+(1,1))) => wait(2) => 2

catch(x)
若x的名字为err，将其名字替换为cerr。返回x。
该函数的解释优先于err的扩散。详见err。
例子：
catch(/(1,0)) => catch(err(from_/:division_by_zero)) => cerr(from_/:division_by_zero)

raise(x)
将自己的名字改为err，返回自己。
例子：
raise(mess) => err(mess)

以下是约定的没有定义的函数：

True、False 作为if中的cond及一系列比较函数的返回值。

1、2等数字 作为数字运算函数的参数和部分函数的索引。

None 作为len函数在参数没有参数时的返回值。

list 作为列表用于装载函数。

lam(args...,body)
在call中作为第一个参数时，可以根据call中的其它参数对body内的和args同名的函数进行替换。详见call和替换。

err
当一个被函数被解释，且内部包含err时，它会跳过解释，变为那个err。
例子：
+(1,err) => err

cerr
作为被catch捕获的err。

以下是其它补充内容：

特殊写法：
当一个有参数的函数f(x)后面加上括号和参数，形成类似f(x)(y)这样的形式时，这种写法会被转化为call(f(x),y)。

替换：
替换需要一组替换目标名(args)，和对应的一组替换物函数(actargs)，以及一个被替换的函数。
替换时，会遍历被替换的函数里的每一个函数，找到每一个名字与args匹配的函数。
以下假设名字与args匹配的函数为arg'：
当arg'没有参数时，会直接替换成对应actargs。
当arg'有参数时，且actargs没有参数时，会把arg'的名字替换为actargs的名字。
当arg'有参数时，且actargs有参数时，会把arg'替换为call(actargs,arg'的参数)。
当arg'是def、defn、lam、rp、rpall函数中的args...或name参数时，不会对它以及这个函数的内部进行替换。
注意，在对rp、rpall解释时，可以对def、defn、lam、rp、rpall函数中的args...或name参数进行替换。
例子：
lam(x,y,lam(y,x,-(x,y))(x,y))(2,3) => call(lam(y,x,-(x,y)),2,3) => -(3,2) => 1
rpall(x,y,lam(x,y,+(x,y))) => lam(y,y,+(y,y))