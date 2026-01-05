/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import ScreenContainer from '@ui/components/screen-container'
import { GetComponent } from '../../../app/element-loaders'
import { Component } from '@src/app/element-loaders/collections/component-collection'
import { Suspense } from 'react'

const Container = styled.div`
  height: 100%;
  display: flex;
  flex-direction: column;
  gap: 1rem;
`

const TestScreen = () => {
  // const TestComponent = LoadComponent('TestComponent')

  const LazyComponent = GetComponent(Component.TestComponent)

  return (
    <ScreenContainer isAdmin={true}>
      <Container>
        <Suspense>
          <LazyComponent />
        </Suspense>
      </Container>
    </ScreenContainer>
  )
}

export default TestScreen
