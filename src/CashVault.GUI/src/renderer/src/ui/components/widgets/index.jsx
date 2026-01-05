/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import styled from '@emotion/styled'
import { Suspense, useMemo } from 'react'
import { GetComponent } from '@src/app/element-loaders'
import WidgetSkeleton from './widget/skeleton'

const Container = styled.div`
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
  width: 100%;

  & > div {
    opacity: 0;
    animation: slideIn 0.5s ease forwards;
    animation-delay: calc(0.2s * var(--i));
  }

  & > div:nth-of-type(n) {
    animation-delay: calc(0.2s * (var(--i)));
  }

  @keyframes slideIn {
    from {
      opacity: 0;
      transform: translateX(-2.5rem);
    }
    to {
      opacity: 1;
      transform: translateX(0);
    }
  }
`

const WidgetsContainer = ({ widgets = [], isLoading = false, className, ...rest }) => {
  if (isLoading === true) {
    return <WidgetSkeleton />
  }

  if (!widgets || widgets.length < 1) {
    return <></>
  }

  const widgetComponents = useMemo(() => {
    if (!widgets || widgets.length < 1) {
      return []
    }

    return widgets.map((x, i) => {
      const WidgetComponent = GetComponent(x.code)

      return (
        <Suspense key={`widget-${i}`} fallback={<>Loading</>}>
          <WidgetComponent size={x.size} gap={'1rem'} style={{ '--i': i }} {...rest} />
        </Suspense>
      )
    })
  }, [widgets])

  return <Container className={className}>{widgetComponents}</Container>
}

export default WidgetsContainer
